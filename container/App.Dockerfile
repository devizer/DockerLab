FROM microsoft/dotnet:2.0.3-runtime-deps-jessie
WORKDIR /app
COPY . /app
ENTRYPOINT [ "./TheApp", "-MySQL=oops" ]
