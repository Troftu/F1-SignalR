using Microsoft.AspNet.SignalR.Client;
using System.Net;

var quitTokenSource = new CancellationTokenSource();
Console.CancelKeyPress += (sender, args) => quitTokenSource.Cancel();

Console.WriteLine("Starting");
using var connection = new HubConnection("https://livetiming.formula1.com/signalr");
connection.CookieContainer = new CookieContainer();

connection.Received += data => Console.WriteLine("Recv: " + data);
connection.StateChanged += change => Console.WriteLine("Connection state changed to " + change.NewState);

connection.ConnectionSlow += () => Console.WriteLine("Connection is slow");
connection.Error += ex => Console.WriteLine("The following connection error was encountered: " + ex.Message);
connection.Reconnecting += () => Console.WriteLine("Reconnecting...");
connection.Reconnected += () => Console.WriteLine("Reconnected");

var streamingHub = connection.CreateHubProxy("Streaming");
await connection.Start();

await streamingHub.Invoke("Subscribe", new List<string> { "Heartbeat", "CarData.z", "Position.z",
    "ExtrapolatedClock", "TopThree", "RcmSeries",
    "TimingStats", "TimingAppData",
    "WeatherData", "TrackStatus", "DriverList",
    "RaceControlMessages", "SessionInfo",
    "SessionData", "LapCount", "TimingData"});

quitTokenSource.Token.WaitHandle.WaitOne();
connection.Stop();
Console.WriteLine("Quitting");