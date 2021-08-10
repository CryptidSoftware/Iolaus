# Iolaus
Iolaus is a .NET library that uses pattern matching to route requests to destinations while being transport independant; because Iolaus routes messages based on their properties rather than an address, it's easy to redirect messages to new destinations without changing the sender.
# NuGet Installation
Coming Soon
# Overview
Iolaus leverages configurations to route messages to destinations in a transport independent way. 
## Configuration
Iolaus configurations are made up of two parts: `Pattern` information and `Route` information. The `Pattern` section contains JSONPath expressions that are used to determine which messages match a given configuration. The `Route` section contains information about where/how matching message should be delivered.
``` json
[
    {
        "Pattern": {
            "$.type": "dice",
            "$.cmd": "roll"
        },
        "Route" : {
            "Type": "NATS",
            "Subject": "dice.roll"
        }
    },
    {
        "Pattern": {
            "$.type": "dice",
            "$.cmd": "roll",
            "$.stats": true
        },
        "Route" : {
            "Type": "HTTP",
            "Url": "http://localhost:6000/dice/roll"
        }
    }
]
```
For example, for the configuration above, this message would be a match for the first pattern and would be sent to the NATS destination:
``` json
{
    "type": "dice",
    "cmd": "roll"
}
```
Whereas this message would match the second condition:
``` json
{
    "type": "dice",
    "cmd": "roll",
    "stats": true
}
```
You may notice that the two messages are almost identical. Iolaus picks the best match from available configurations and sends the message to the appropriate route (best is defined as number of patterns matched). If a message does not match on all of the patterns defined, it will not be considered a match. This is why the first message will not be considered a match for the second configuration.
## Router
Iolaus uses routes to control how messages are handled. A route consists of a `Pattern` and a function that takes a `Message` and returns an `IObservable<Option<Message>>>`.
``` C#
//example handler for responding to a message
var route = (Message m) => Observable.Return<Option<Message>>(Message.Parse("{\"test\":\"message\"}"));
```
Due to how routes are defined, new `Message` handlers can be easily created. This allows for a great deal of flexibility.
## Route Registry
To register routes for messages, a `RouteRegistry` can be added to IServiceCollection with `AddRouteRegistry`. `RouteRegistry` is used to map from a type string to a function. The route configuration can then map from a configuration to a message handler.
``` C#
services.AddRouteRegistry(r => {
    r.Add("NATS", Iolaus.Nats.NatsRoute.Route);
    r.Add("HTTP", Iolaus.Http.HttpRoute.Route);
});
```

## Example
An example of Iolaus in action can be found here: https://gitlab.com/data54/iolausdemo/-/tree/master
# References
* JSONPath: https://support.smartbear.com/alertsite/docs/monitors/api/endpoint/jsonpath.html
* Newtonsoft.Json: https://www.newtonsoft.com/json/help/html/Introduction.htm