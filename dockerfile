FROM dhi.io/dotnet:10-sdk
WORKDIR /back-office
COPY . .
RUN dotnet publish -c Release -o out
CMD ["dotnet", "out/Back-office.dll"]
EXPOSE 5000
