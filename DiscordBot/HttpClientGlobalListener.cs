using System.Diagnostics;
using Newtonsoft.Json;

public class HttpClientGlobalListener : IObserver<DiagnosticListener>
{
    private readonly HttpClientInterceptor _interceptor = new HttpClientInterceptor();

    public void OnCompleted() { }

    public void OnError(Exception error) { }

    public void OnNext(DiagnosticListener listener)
    {
        listener.Subscribe(_interceptor);
    }
}

public class HttpClientInterceptor : IObserver<KeyValuePair<string, object>>
{
    public void OnCompleted() { }

    public void OnError(Exception error) { }

    public void OnNext(KeyValuePair<string, object> value)
    {
        //Debug.Print("Key: " + value.Key);
        if (value.Key == "System.Net.Http.HttpRequestOut.Start")
        {
            var requestObj = JsonConvert.SerializeObject(value.Value);
            Debug.WriteLine($"Request: {requestObj}");
        }
        else if (value.Key == "System.Net.Http.HttpRequestOut.Stop")
        {
            var requestObj = JsonConvert.SerializeObject(value.Value);
            Debug.WriteLine($"Response: {requestObj}");
        }
    }
}