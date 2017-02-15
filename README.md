# generateToken-c-sharp
Sample C# application to generate room tokens for Vidyo.io

To build, open GenerateToken.sln in Visual Studio (created in VS 2013) and build.

This will create GenerateToken/GenerateToken/bin/Debug/GenerateToken.exe

Generate tokens using your appID and developer key from vidyo.io:

    GenerateToken.exe --key=rUlaMASgt1Byi4Kp3sKYDeQzo --appID=ApplicationID --userName=user1 --expiresInSecs=10000

Produces output:    
    
    Setting key           :  rUlaMASgt1Byi4Kp3sKYDeQzo
    Setting appID         :  ApplicationID
    Setting userName      :  user1
    Setting expiresInSecs :  10000
    Generating Token...
    cHJvddfa3r6uAHVzZXIxQEFwcGxpY2F0aW9uSUQANjM2NTI2MDI3ODUAAGQzMDkxMjA5NjFmMGYxMjFkM2FlZjQxMzJkNmRiNTdkMTA5MDU0MGU4ZWZmNjYxMzlhOTUyMzJiODA0MGViOWU5MjI3OTQ3N2MwYWUzODQ3Y2NiYmJiYTNhZDc5OTdkOA==
