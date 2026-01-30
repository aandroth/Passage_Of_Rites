using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor.VersionControl;
using UnityEngine;
using System.Threading.Tasks; // Add this using directive



public class FakeServer : MonoBehaviour
{
    public string m_serverPath = "C:\\Users\\aandr\\Desktop\\Passage_of_Rites_Server\\";
    public string m_serverStartPs1File = "ServerStart.ps1";
    public string m_serverKillCommand = "";
    public Backend m_backend = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_backend = FindAnyObjectByType<Backend>();
    }

    public async void StartServer()
    {
        UnityEngine.Debug.Log($"Starting fake server with command: {m_serverStartPs1File} in path: {m_serverPath}");
        RunPS();
        await ConnectToServer();
    }

    public System.Threading.Tasks.Task RunPS()
    {
        return System.Threading.Tasks.Task.Run(() =>
        {
            using (Process p = new Process())
            {
                var ps1File = Path.Combine(m_serverPath, m_serverStartPs1File);
                p.StartInfo.FileName = "powershell.exe";
                p.StartInfo.Arguments = $"-NoProfile -ExecutionPolicy ByPass -File \"{ps1File}\"";
                p.StartInfo.WorkingDirectory = m_serverPath;
                p.StartInfo.CreateNoWindow = false;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.OutputDataReceived += (sender, args) => UnityEngine.Debug.LogWarning(args.Data);
                p.ErrorDataReceived += (sender, args) => UnityEngine.Debug.LogError(args.Data);
                p.Start();
                p.WaitForExit(60000); // Wait for 1 second to let the server start
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                UnityEngine.Debug.Log($"Started fake server");
            }
        });
    }

    public async System.Threading.Tasks.Task ConnectToServer()
    {
        await System.Threading.Tasks.Task.Delay(4000); // Wait for 2 seconds to let the server start
        UnityEngine.Debug.Log($"Connecting to fake server");

        await m_backend.StartWebSocketConnection();
    }
}
