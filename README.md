# ChromiumBug
Sample project to illistrate reproduction steps for https://bugs.chromium.org/p/chromium/issues/detail?id=696204

# Steps to reproduce:
* Uncomment [line 52](https://github.com/daniefer/ChromiumBug/blob/master/ExampleServer/Startup.cs#L52) in the Startup.cs
* Start the project up.
* Visit https://localhost:5001
* Refresh several time and see the Cookie is getting set each time.
* Visit http://localhost:5000
* Notice the Cookie does not get updated even after several refreshes.
* Stop the project and comment out [line 52](https://github.com/daniefer/ChromiumBug/blob/master/ExampleServer/Startup.cs#L52) in the Startup.cs again.
* Start up the project.
* Visit http://localhost:5000
* Notice the Cookie does not get updated even after several refreshes.
Visit https://localhost:5001
* Notice the Cookie value starts updating again on subsequent refreshes.
* Visit http://localhost:5000
* Notice the Cookie NOW get updated each refresh.

To trigger this problem again, visit the https site again with the secure cookie option provided. Once that occures, Set-Cookie requests will not be respected over http with or without the secure cookie option.
