# FROM microsoft/dotnet:2.0.7-runtime-deps-jessie
FROM microsoft/dotnet:2.2-runtime
# FROM debian
WORKDIR /WaitFor
COPY . /WaitFor
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
ENTRYPOINT [ "./WaitFor" ]
