/*
Copyright 2022 Boonebytes

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

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