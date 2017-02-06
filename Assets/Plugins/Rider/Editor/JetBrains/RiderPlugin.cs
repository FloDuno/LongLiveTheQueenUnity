using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Plugins.Editor.JetBrains
{
  [InitializeOnLoad]
  public static class RiderPlugin
  {
    private static bool _initialized;

    private static string _slnFile;

    private static string DefaultApp
    {
      get { return EditorPrefs.GetString("kScriptsDefaultApp"); }
    }

    private static readonly int UnityProcessId = Process.GetCurrentProcess().Id;
    private static readonly string UnityVersion = Application.unityVersion;

    public static bool TargetFrameworkVersion45
    {
      get { return EditorPrefs.GetBool("Rider_TargetFrameworkVersion45", true); }
      set { EditorPrefs.SetBool("Rider_TargetFrameworkVersion45", value); }
    }

    internal static bool Enabled
    {
      get
      {
        return !string.IsNullOrEmpty(DefaultApp) && DefaultApp.ToLower().Contains("rider");
      }
    }

    static RiderPlugin()
    {
      if (Enabled)
      {
        InitRiderPlugin();
      }
    }

    private static void InitRiderPlugin()
    {
      FileInfo _riderFileInfo = new FileInfo(DefaultApp);

      string _newPath = _riderFileInfo.FullName;
      // try to search the new version

      if (!_riderFileInfo.Exists)
      {
        switch (_riderFileInfo.Extension)
        {
          case ".exe":
          {
            FileInfo[] _possibleNew =
              _riderFileInfo.Directory.Parent.Parent.GetDirectories("*ider*")
                .SelectMany(_a => _a.GetDirectories("bin"))
                .SelectMany(_a => _a.GetFiles(_riderFileInfo.Name))
                .ToArray();
            if (_possibleNew.Length > 0)
              _newPath = _possibleNew.OrderBy(_a => _a.LastWriteTime).Last().FullName;
            break;
          }
        }
        if (_newPath != _riderFileInfo.FullName)
        {
          Log(string.Format("Update {0} to {1}", _riderFileInfo.FullName, _newPath));
          EditorPrefs.SetString("kScriptsDefaultApp", _newPath);
        }
      }

      string _projectDirectory = Directory.GetParent(Application.dataPath).FullName;
      string _projectName = Path.GetFileName(_projectDirectory);
      _slnFile = Path.Combine(_projectDirectory, string.Format("{0}.sln", _projectName));
      UpdateUnitySettings(_slnFile);

      // will be used by dependent Rider to provide Denug Configuration and other features
      Environment.SetEnvironmentVariable("unityProcessId", UnityProcessId.ToString());
      Environment.SetEnvironmentVariable("unityVersion", UnityVersion);

      _initialized = true;
    }

    /// <summary>
    /// Helps to open xml and txt files at least on Windows
    /// </summary>
    /// <param name="slnFile"></param>
    private static void UpdateUnitySettings(string _slnFile)
    {
      try
      {
        EditorPrefs.SetString("kScriptEditorArgs", string.Format("{0}{1}{0} {0}$(File){0}", "\"", _slnFile));
      }
      catch (Exception _e)
      {
        Log("Exception on updating kScriptEditorArgs: " + _e.Message);
      }
    }

    /// <summary>
    /// Asset Open Callback (from Unity)
    /// </summary>
    /// <remarks>
    /// Called when Unity is about to open an asset.
    /// </remarks>
    [OnOpenAsset]
    private static bool OnOpenedAsset(int _instanceID, int _line)
    {
      if (Enabled)
      {
        if (!_initialized)
        {
          // make sure the plugin was initialized first.
          // this can happen in case "Rider" was set as the default scripting app only after this plugin was imported.
          InitRiderPlugin();
          RiderAssetPostprocessor.OnGeneratedCSProjectFiles();
        }

        string _appPath = Path.GetDirectoryName(Application.dataPath);

        // determine asset that has been double clicked in the project view
        Object _selected = EditorUtility.InstanceIDToObject(_instanceID);

        if (_selected.GetType().ToString() == "UnityEditor.MonoScript" ||
            _selected.GetType().ToString() == "UnityEngine.Shader")
        {
          SyncSolution(); // added to handle opening file, which was just recently created.
          string _assetFilePath = Path.Combine(_appPath, AssetDatabase.GetAssetPath(_selected));
          if (!CallUdpRider(_line, _slnFile, _assetFilePath))
          {
              string _args = string.Format("{0}{1}{0} -l {2} {0}{3}{0}", "\"", _slnFile, _line, _assetFilePath);
              CallRider(DefaultApp, _args);
          }
          return true;
        }
      }
      return false;
    }

    private static bool CallUdpRider(int _line, string _slnPath, string _filePath)
    {
      Log(string.Format("CallUDPRider({0} {1} {2} {3} {4})", _line, _slnPath, _filePath, UnityProcessId, UnityVersion));
      using (Socket _sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
      {
        try
        {
          _sock.ReceiveTimeout = 10000;

          IPAddress _serverAddr = IPAddress.Parse("127.0.0.1");
          IPEndPoint _endPoint = new IPEndPoint(_serverAddr, 11234);

          string _text = _line + "\r\n" + _slnPath + "\r\n" + _filePath + "\r\n"+UnityProcessId+ "\r\n"+UnityVersion+ "\r\n";
          byte[] _sendBuffer = Encoding.ASCII.GetBytes(_text);
          _sock.SendTo(_sendBuffer, _endPoint);

          byte[] _rcvBuffer = new byte[1024];

          // Poll the socket for reception with a 10 ms timeout.
          if (!_sock.Poll(10000, SelectMode.SelectRead))
          {
            throw new TimeoutException();
          }

          int _bytesRec = _sock.Receive(_rcvBuffer); // This call will not block
          string _status = Encoding.ASCII.GetString(_rcvBuffer, 0, _bytesRec);
          if (_status == "ok")
          {
            ActivateWindow(new FileInfo(DefaultApp).FullName);
            return true;
          }
        }
        catch (Exception)
        {
          //error Timed out
          Log("Socket error or no response. Have you installed RiderUnity3DConnector in Rider?");
        }
      }
      return false;
    }

    private static void CallRider(string _riderPath, string _args)
    {
      FileInfo _riderFileInfo = new FileInfo(_riderPath);
      bool _macOsVersion = _riderFileInfo.Extension == ".app";
      bool _riderExists = _macOsVersion ? new DirectoryInfo(_riderPath).Exists : _riderFileInfo.Exists;
      
      if (!_riderExists)
      {
        EditorUtility.DisplayDialog("Rider executable not found", "Please update 'External Script Editor' path to JetBrains Rider.", "OK");
      }

      Process _proc = new Process();
      if (_macOsVersion)
      {
        _proc.StartInfo.FileName = "open";
        _proc.StartInfo.Arguments = string.Format("-n {0}{1}{0} --args {2}", "\"", "/" + _riderPath, _args);
        Log(_proc.StartInfo.FileName + " " + _proc.StartInfo.Arguments);
      }
      else
      {
        _proc.StartInfo.FileName = _riderPath;
        _proc.StartInfo.Arguments = _args;
        Log("\"" + _proc.StartInfo.FileName + "\"" + " " + _proc.StartInfo.Arguments);
      }

      _proc.StartInfo.UseShellExecute = false;
      _proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
      _proc.StartInfo.CreateNoWindow = true;
      _proc.StartInfo.RedirectStandardOutput = true;
      _proc.Start();

      ActivateWindow(_riderPath);
    }

    private static void ActivateWindow(string _riderPath)
    {
      if (new FileInfo(_riderPath).Extension == ".exe")
      {
        try
        {
          Process _process = Process.GetProcesses().FirstOrDefault(_p =>
          {
            string _processName;
            try
            {
              _processName = _p.ProcessName; // some processes like kaspersky antivirus throw exception on attempt to get ProcessName
            }
            catch (Exception)
            {
              return false;
            }

            return !_p.HasExited && _processName.ToLower().Contains("rider");
          });
          if (_process != null)
          {
            // Collect top level windows
            List<IntPtr> _topLevelWindows = User32Dll.GetTopLevelWindowHandles();
            // Get process main window title
            IntPtr _windowHandle = _topLevelWindows.FirstOrDefault(_hwnd => User32Dll.GetWindowProcessId(_hwnd) == _process.Id);
            if (_windowHandle != IntPtr.Zero)
              User32Dll.SetForegroundWindow(_windowHandle);
          }
        }
        catch (Exception _e)
        {
          Log("Exception on ActivateWindow: " + _e);
        }
      }
    }

    [MenuItem("Assets/Open C# Project in Rider", false, 1000)]
    private static void MenuOpenProject()
    {
      // Force the project files to be sync
      SyncSolution();

      // Load Project
      CallRider(DefaultApp, string.Format("{0}{1}{0}", "\"", _slnFile));
    }

    [MenuItem("Assets/Open C# Project in Rider", true, 1000)]
    private static bool ValidateMenuOpenProject()
    {
      return Enabled;
    }

    /// <summary>
    /// Force Unity To Write Project File
    /// </summary>
    private static void SyncSolution()
    {
      Type _ = Type.GetType("UnityEditor.SyncVS,UnityEditor");
      MethodInfo _syncSolution = _.GetMethod("SyncSolution",
        BindingFlags.Public | BindingFlags.Static);
      _syncSolution.Invoke(null, null);
    }

    public static void Log(object _message)
    {
      Debug.Log("[Rider] " + _message);
    }

    /// <summary>
    /// JetBrains Rider Integration Preferences Item
    /// </summary>
    /// <remarks>
    /// Contains all 3 toggles: Enable/Disable; Debug On/Off; Writing Launch File On/Off
    /// </remarks>
    [PreferenceItem("Rider")]
    private static void RiderPreferencesItem()
    {
      EditorGUILayout.BeginVertical();

      string _url = "https://github.com/JetBrains/Unity3dRider";
      if (GUILayout.Button(_url))
      {
        Application.OpenURL(_url);
      }

      EditorGUI.BeginChangeCheck();

      string _help = @"For now target 4.5 is strongly recommended.
 - Without 4.5:
    - Rider will fail to resolve System.Linq on Mac/Linux
    - Rider will fail to resolve Firebase Analytics.
 - With 4.5 Rider will show ambiguos references in UniRx.
All thouse problems will go away after Unity upgrades to mono4.";
      TargetFrameworkVersion45 =
        EditorGUILayout.Toggle(
          new GUIContent("TargetFrameworkVersion 4.5",
            _help), TargetFrameworkVersion45);
      EditorGUILayout.HelpBox(_help, MessageType.None);

      EditorGUI.EndChangeCheck();
    }

      private static class User32Dll
    {

      /// <summary>
      /// Gets the ID of the process that owns the window.
      /// Note that creating a <see cref="Process"/> wrapper for that is very expensive because it causes an enumeration of all the system processes to happen.
      /// </summary>
      public static int GetWindowProcessId(IntPtr _hwnd)
      {
        uint _dwProcessId;
        GetWindowThreadProcessId(_hwnd, out _dwProcessId);
        return unchecked((int) _dwProcessId);
      }

      /// <summary>
      /// Lists the handles of all the top-level windows currently available in the system.
      /// </summary>
      public static List<IntPtr> GetTopLevelWindowHandles()
      {
        List<IntPtr> _retval = new List<IntPtr>();
        EnumWindowsProc _callback = (_hwnd, _param) =>
        {
          _retval.Add(_hwnd);
          return 1;
        };
        EnumWindows(Marshal.GetFunctionPointerForDelegate(_callback), IntPtr.Zero);
        GC.KeepAlive(_callback);
        return _retval;
      }

      public delegate Int32 EnumWindowsProc(IntPtr _hwnd, IntPtr _lParam);

      [DllImport("user32.dll", CharSet = CharSet.Unicode, PreserveSig = true, SetLastError = true, ExactSpelling = true)]
      public static extern Int32 EnumWindows(IntPtr _lpEnumFunc, IntPtr _lParam);

      [DllImport("user32.dll", SetLastError = true)]
      private static extern uint GetWindowThreadProcessId(IntPtr _hWnd, out uint _lpdwProcessId);

      [DllImport("user32.dll", CharSet = CharSet.Unicode, PreserveSig = true, SetLastError = true, ExactSpelling = true)]
      public static extern Int32 SetForegroundWindow(IntPtr _hWnd);
    }
  }
}

// Developed using JetBrains Rider =)
