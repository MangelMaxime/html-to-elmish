# Html to Elmish [![Build Status](https://dev.azure.com/MangelMaximeGithub/html-to-elmish/_apis/build/status/MangelMaxime.html-to-elmish?branchName=master)](https://dev.azure.com/MangelMaximeGithub/html-to-elmish/_build/latest?definitionId=1&branchName=master)

**As an OpenSource maintainer, my time is limited for this reason, this project is now consider completed. Only bugs will now be fixed**

## How to build ?

```shell
./fake.sh run build.fsx
```

The file needed to host the WebApp are in `./src/WebApp/output` folder.

## How to debug the WebApp ?

```shell
./fake.sh run build.fsx -t Watch
```

Browser to [http://localhost:8080](http://localhost:8080), the changes are applies directly to your application on save.

## How run the HtmlConverter tests ?

```shell
./fake.sh run build.fsx -t RunLiveTests
```

Browser to [http://localhost:8080](http://localhost:8080), you browser is reload each time you modify HtmlConverter or the tests.
