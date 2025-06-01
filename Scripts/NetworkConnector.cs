using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkConnector : MonoBehaviour
{
    [Header("UI")]
    public Text ipDisplayText;
    public InputField ipInputField;

    [Header("Scene")]
    public string gameplaySceneName = "Main_Gameplay_Multi";

    private TcpClient tcpClient;
    private TcpListener tcpListener;
    private NetworkStream stream;
    private Thread tcpThread;
    private Thread listenThread;

    public static Action<string> OnMessageReceived;

   
    private UdpClient udpSender;
    private UdpClient udpReceiver;
    private Thread udpListenThread;
    public static Action<string> OnUDPMessageReceived;

    private const int tcpPort = 5555;
    private const int udpPort = 5556;

    public bool isHost = false;
    private string remoteIP;

    public void StartHost()
    {
        isHost = true;

        string localIP = GetLocalIPAddress();
        ipDisplayText.text = $"Your IP: {localIP}";
        Debug.Log("Set IP text to UI: " + ipDisplayText.text);

        tcpListener = new TcpListener(IPAddress.Any, tcpPort);
        tcpListener.Start();

        tcpThread = new Thread(() =>
        {
            tcpClient = tcpListener.AcceptTcpClient();
            Debug.Log(" Client connected!");

            stream = tcpClient.GetStream();
            StartTCPListening();

            remoteIP = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString();
            StartUDP(remoteIP);

            SceneManager.LoadScene(gameplaySceneName);
        });

        tcpThread.IsBackground = true;
        tcpThread.Start();
    }

   
    public void ConnectToHost()
    {
        isHost = false;

        remoteIP = ipInputField.text.Trim();
        tcpClient = new TcpClient();

        try
        {
            tcpClient.Connect(remoteIP, tcpPort);
            Debug.Log(" Connected to host!");

            stream = tcpClient.GetStream();
            StartTCPListening();

            StartUDP(remoteIP);

            SceneManager.LoadScene(gameplaySceneName);
        }
        catch (Exception ex)
        {
            Debug.LogError(" Connection failed: " + ex.Message);
        }
    }

  
    void StartTCPListening()
    {
        listenThread = new Thread(() =>
        {
            byte[] buffer = new byte[1024];

            while (tcpClient.Connected)
            {
                try
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Debug.Log(" TCP Received: " + message);
                        OnMessageReceived?.Invoke(message);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("TCP listening error: " + ex.Message);
                    break;
                }
            }
        });

        listenThread.IsBackground = true;
        listenThread.Start();
    }

    public void SendMessageToPeer(string message)
    {
        if (tcpClient != null && tcpClient.Connected)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
            stream.Flush();
            Debug.Log(" TCP Sent: " + message);
        }
    }

    
    void StartUDP(string ip)
    {
        try
        {
            udpSender = new UdpClient();
            udpSender.Connect(ip, udpPort);

            udpReceiver = new UdpClient(udpPort);

            udpListenThread = new Thread(() =>
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, udpPort);

                while (true)
                {
                    try
                    {
                        byte[] data = udpReceiver.Receive(ref remoteEP);
                        string msg = Encoding.UTF8.GetString(data);
                        Debug.Log(" UDP Received: " + msg);
                        OnUDPMessageReceived?.Invoke(msg);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("UDP listen error: " + ex.Message);
                        break;
                    }
                }
            });

            udpListenThread.IsBackground = true;
            udpListenThread.Start();
        }
        catch (Exception ex)
        {
            Debug.LogError("UDP init error: " + ex.Message);
        }
    }

    
    public void SendUDP(string message)
    {
        if (udpSender != null)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            udpSender.Send(data, data.Length);
            Debug.Log(" UDP Sent: " + message);
        }
    }

   
    private string GetLocalIPAddress()
    {
        string localIP = "Not found";
        var host = Dns.GetHostEntry(Dns.GetHostName());

        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }

        return localIP;
    }

    void Awake()
    {
        
        if (FindObjectsOfType<NetworkConnector>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }


    void OnApplicationQuit()
    {
        try
        {
            listenThread?.Abort();
            tcpThread?.Abort();
            stream?.Close();
            tcpClient?.Close();
            tcpListener?.Stop();

            udpListenThread?.Abort();
            udpSender?.Close();
            udpReceiver?.Close();
        }
        catch (Exception e)
        {
            Debug.LogWarning("Cleanup failed: " + e.Message);
        }
    }
}
