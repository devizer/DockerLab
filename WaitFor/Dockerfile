# FROM microsoft/dotnet:2.0.7-runtime-deps-jessie
FROM microsoft/dotnet:2.1-runtime-alpine
# FROM debian
WORKDIR /WaitFor
COPY . /WaitFor
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
ENTRYPOINT [ "dotnet","./WaitFor.dll" ]
