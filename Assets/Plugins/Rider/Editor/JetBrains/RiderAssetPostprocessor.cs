using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

namespace Plugins.Editor.JetBrains
{
  public class RiderAssetPostprocessor : AssetPostprocessor
  {
    private const string UnityPlayerProjectName = "Assembly-CSharp.csproj";
    private const string UnityEditorProjectName = "Assembly-CSharp-Editor.csproj";
    private const string UnityUnsafeKeyword = "-unsafe";
    private const string UnityDefineKeyword = "-define:";
    private const string PlayerProjectManualConfigRelativeFilePath = "smcs.rsp";
    private static readonly string  PlayerProjectManualConfigAbsoluteFilePath
      = Path.Combine(Application.dataPath, PlayerProjectManualConfigRelativeFilePath);
    private const string EditorProjectManualConfigRelativeFilePath = "gmcs.rsp";
    private static readonly string  EditorProjectManualConfigAbsoluteFilePath
      = Path.Combine(Application.dataPath, EditorProjectManualConfigRelativeFilePath);

    public static void OnGeneratedCSProjectFiles()
    {
      if (!RiderPlugin.Enabled)
        return;
      string _currentDirectory = Directory.GetCurrentDirectory();
      string[] _projectFiles = Directory.GetFiles(_currentDirectory, "*.csproj");

      foreach (string _file in _projectFiles)
      {
        UpgradeProjectFile(_file);
      }

      string _slnFile = Directory.GetFiles(_currentDirectory, "*.sln").First();
      RiderPlugin.Log(string.Format("Post-processing {0}", _slnFile));
      string _content = File.ReadAllText(_slnFile);
      string[] _lines = _content.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
      StringBuilder _sb = new StringBuilder();
      foreach (string _line in _lines)
      {
        if (_line.StartsWith("Project("))
        {
          MatchCollection _mc = Regex.Matches(_line, "\"([^\"]*)\"");
          //RiderPlugin.Log(mc[2].Value);
          _sb.Append(_line.Replace(_mc[1].Value, GetFileNameWithoutExtension(_mc[2].Value)+"\""));
        }
        else
        {
          _sb.Append(_line);
        }
        _sb.Append(Environment.NewLine);
      }
      File.WriteAllText(_slnFile,_sb.ToString());
    }

    private static string GetFileNameWithoutExtension(string _path)
    {
      if (_path == null)
        return null;
      int _length;
      if ((_length = _path.LastIndexOf('.')) == -1)
        return _path;
      return _path.Substring(0, _length);
    }

    private static void UpgradeProjectFile(string _projectFile)
    {
      RiderPlugin.Log(string.Format("Post-processing {0}", _projectFile));
      XDocument _doc = XDocument.Load(_projectFile);
      XElement _projectContentElement = _doc.Root;
      XNamespace _xmlns = _projectContentElement.Name.NamespaceName; // do not use var

      FixTargetFrameworkVersion(_projectContentElement, _xmlns);
      SetLangVersion(_projectContentElement, _xmlns);
      SetManuallyDefinedComilingSettings(_projectFile, _projectContentElement, _xmlns);

      SetXCodeDllReference("UnityEditor.iOS.Extensions.Xcode.dll", _xmlns, _projectContentElement);
      SetXCodeDllReference("UnityEditor.iOS.Extensions.Common.dll", _xmlns, _projectContentElement);

      _doc.Save(_projectFile);
    }

    private static void SetManuallyDefinedComilingSettings(string _projectFile, XElement _projectContentElement, XNamespace _xmlns)
    {
      string _configPath;

      if (IsPlayerProjectFile(_projectFile))
        _configPath = PlayerProjectManualConfigAbsoluteFilePath;
      else if (IsEditorProjectFile(_projectFile))
        _configPath = EditorProjectManualConfigAbsoluteFilePath;
      else
        _configPath = null;

      if(!string.IsNullOrEmpty(_configPath))
        ApplyManualCompilingSettings(_configPath
          , _projectContentElement
          , _xmlns);
    }

    private static void ApplyManualCompilingSettings(string _configFilePath, XElement _projectContentElement, XNamespace _xmlns)
    {
      if (File.Exists(_configFilePath))
      {
        string _configText = File.ReadAllText(_configFilePath);
        if (_configText.Contains(UnityUnsafeKeyword))
        {
          // Add AllowUnsafeBlocks to the .csproj. Unity doesn't generate it (although VSTU does).
          // Strictly necessary to compile unsafe code
          ApplyAllowUnsafeBlocks(_projectContentElement, _xmlns);
        }
        if (_configText.Contains(UnityDefineKeyword))
        {
          // defines could be
          // 1) -define:DEFINE1,DEFINE2
          // 2) -define:DEFINE1;DEFINE2
          // 3) -define:DEFINE1 -define:DEFINE2
          // 4) -define:DEFINE1,DEFINE2;DEFINE3
          // tested on "-define:DEF1;DEF2 -define:DEF3,DEF4;DEFFFF \n -define:DEF5"
          // result: DEF1, DEF2, DEF3, DEF4, DEFFFF, DEF5

          List<string> _definesList = new List<string>();
          string[] _compileFlags = _configText.Split(' ', '\n');
          foreach (string _flag in _compileFlags)
          {
            string _f = _flag.Trim();
            if (_f.Contains(UnityDefineKeyword))
            {
              int _defineEndPos = _f.IndexOf(UnityDefineKeyword) + UnityDefineKeyword.Length;
              string _definesSubString = _f.Substring(_defineEndPos,_f.Length - _defineEndPos);
              _definesSubString = _definesSubString.Replace(";", ",");
              _definesList.AddRange(_definesSubString.Split(','));
            }
          }

          //UnityEngine.Debug.Log(string.Join(", ",definesList.ToArray()));
          ApplyCustomDefines(_definesList.ToArray(), _projectContentElement, _xmlns);
        }
      }
    }

    private static void ApplyCustomDefines(string[] _customDefines, XElement _projectContentElement, XNamespace _xmlns)
    {
      string _definesString = string.Join(";", _customDefines);

      XElement _defineConstants = _projectContentElement
        .Elements(_xmlns+"PropertyGroup")
        .Elements(_xmlns+"DefineConstants")
        .FirstOrDefault(_definesConsts=> !string.IsNullOrEmpty(_definesConsts.Value));

      if (_defineConstants != null)
      {
        _defineConstants.SetValue(_defineConstants.Value + ";" + _definesString);
      }
    }

    private static void ApplyAllowUnsafeBlocks(XElement _projectContentElement, XNamespace _xmlns)
    {
      _projectContentElement.AddFirst(
        new XElement(_xmlns + "PropertyGroup", new XElement(_xmlns + "AllowUnsafeBlocks", true)));
    }

    private static bool IsPlayerProjectFile(string _projectFile)
    {
      return Path.GetFileName(_projectFile) == UnityPlayerProjectName;
    }

    private static bool IsEditorProjectFile(string _projectFile)
    {
      return Path.GetFileName(_projectFile) == UnityEditorProjectName;
    }

    // Helps resolve System.Linq under mono 4 - RIDER-573
    private static void FixTargetFrameworkVersion(XElement _projectElement, XNamespace _xmlns)
    {
      if (!RiderPlugin.TargetFrameworkVersion45)
        return;

      XElement _targetFrameworkVersion = _projectElement.Elements(_xmlns + "PropertyGroup").
        Elements(_xmlns + "TargetFrameworkVersion").First();
      Version _version = new Version(_targetFrameworkVersion.Value.Substring(1));
      if (_version < new Version(4, 5))
        _targetFrameworkVersion.SetValue("v4.5");
    }

    private static void SetLangVersion(XElement _projectElement, XNamespace _xmlns)
    {
      // Add LangVersion to the .csproj. Unity doesn't generate it (although VSTU does).
      // Not strictly necessary, as the Unity plugin for Rider will work it out, but setting
      // it makes Rider work if it's not installed.
      _projectElement.AddFirst(new XElement(_xmlns + "PropertyGroup",
        new XElement(_xmlns + "LangVersion", GetLanguageLevel())));
    }

    private static string GetLanguageLevel()
    {
      // https://bitbucket.org/alexzzzz/unity-c-5.0-and-6.0-integration/src
      if (Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "CSharp70Support")))
        return "7";
      if (Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "CSharp60SUpport")))
        return "6";

      // Unity 5.5 supports C# 6, but only when targeting .NET 4.6. The enum doesn't exist pre Unity 5.5
      if ((int)PlayerSettings.apiCompatibilityLevel >= 3)
        return "6";

      return "4";
    }

    private static void SetXCodeDllReference(string _name, XNamespace _xmlns, XElement _projectContentElement)
    {
      string _unityAppBaseFolder = Path.GetDirectoryName(EditorApplication.applicationPath);

      string _xcodeDllPath = Path.Combine(_unityAppBaseFolder, Path.Combine("Data/PlaybackEngines/iOSSupport", _name));
      if (!File.Exists(_xcodeDllPath))
        _xcodeDllPath = Path.Combine(_unityAppBaseFolder, Path.Combine("PlaybackEngines/iOSSupport", _name));

      if (File.Exists(_xcodeDllPath))
      {
        XElement _itemGroup = new XElement(_xmlns + "ItemGroup");
        XElement _reference = new XElement(_xmlns + "Reference");
        _reference.Add(new XAttribute("Include", Path.GetFileNameWithoutExtension(_xcodeDllPath)));
        _reference.Add(new XElement(_xmlns + "HintPath", _xcodeDllPath));
        _itemGroup.Add(_reference);
        _projectContentElement.Add(_itemGroup);
      }
    }
  }
}
