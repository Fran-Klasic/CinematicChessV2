using System.Diagnostics;
using System.IO;

public class StockfishEngine
{
    private Process process;
    private StreamWriter input;
    private StreamReader output;

    public void StartEngine(string path)
    {
        process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };

        process.Start();
        input = process.StandardInput;
        output = process.StandardOutput;

        Send("uci");
        Send("isready");
    }

    public void Send(string command)
    {
        if (process == null || process.HasExited)
            return;

        input.WriteLine(command);
        input.Flush();
    }

    public string ReadLine()
    {
        return output.ReadLine();
    }

    public void Stop()
    {
        if (process == null)
            return;

        try
        {
            if (!process.HasExited)
                process.Kill();
        }
        catch
        {
        }

        process.Dispose();
        process = null;
        input = null;
        output = null;
    }

    public string GetBestMove(string fen, int depth = 15)
    {
        Send($"position fen {fen}");
        Send($"go depth {depth}");

        string line;
        while ((line = ReadLine()) != null)
        {
            if (line.StartsWith("bestmove"))
                return line.Split(' ')[1];
        }

        return null;
    }
}
