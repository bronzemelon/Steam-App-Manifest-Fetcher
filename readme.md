# Steam App Manifest Fetcher

# NOTICE
It seems the Steam has deprecated the API and now recommends developers to use a newer `IStoreService` interface with `GetAppList` as its method. Due to this API requiring an API key and the open source nature of this program, I can't simply put my API key out there. As such, this repository will be archived.

**Welcome to Steam App Manifest Fetcher!**

This is a program that fetches and generates an app manifest based on an app id(s) provided.

It will then place it in your Steam folder and appear in your library.

Just make to restart Steam after this.
> App Manifests generated from this program doesn't mean you own the applications.\
It just appears in your Steam library and will prompt you to buy if you don't own the app.

## Instructions
### Installation
Go to [Releases](https://github.com/bronzemelon/Steam-App-Manifest-Fetcher/releases) and download the appropiate file for your platform.\
Extract the zip folder and run the appropiate executable. It should look like the following.

For Windows\
![](https://private-user-images.githubusercontent.com/52508286/502867402-adc716a7-e214-441f-a11d-4ac88d765b18.png?jwt=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJnaXRodWIuY29tIiwiYXVkIjoicmF3LmdpdGh1YnVzZXJjb250ZW50LmNvbSIsImtleSI6ImtleTUiLCJleHAiOjE3NjA3ODkwNDksIm5iZiI6MTc2MDc4ODc0OSwicGF0aCI6Ii81MjUwODI4Ni81MDI4Njc0MDItYWRjNzE2YTctZTIxNC00NDFmLWExMWQtNGFjODhkNzY1YjE4LnBuZz9YLUFtei1BbGdvcml0aG09QVdTNC1ITUFDLVNIQTI1NiZYLUFtei1DcmVkZW50aWFsPUFLSUFWQ09EWUxTQTUzUFFLNFpBJTJGMjAyNTEwMTglMkZ1cy1lYXN0LTElMkZzMyUyRmF3czRfcmVxdWVzdCZYLUFtei1EYXRlPTIwMjUxMDE4VDExNTkwOVomWC1BbXotRXhwaXJlcz0zMDAmWC1BbXotU2lnbmF0dXJlPWU5MmQ0MDYyN2IyYTI0MTZmZGU0MTIxYzBjNzFkMTMzZmU4MDVkN2UzMGYyYzRmMmJlMjA0ZjU4Yzc5YmU5MjkmWC1BbXotU2lnbmVkSGVhZGVycz1ob3N0In0.X6muBHN7IqtF0pafzSWVG9Z3UXryK6kZh0OgPd4Z9ms)\
For Linux
~~~
$ ./"Steam App Manifest Fetcher"
~~~
### First Launch
~~~
Index: ...
~~~
Type in the index to choose an option.
~~~
1. Generate Single App Manifest
2. Generate Multiple App Manifests
3. Search Applications
4. Update Database
5. Quit
~~~
### Generate Single App Manifest
~~~
App id: ...
~~~
This will fetch an app from Steam using an app id provided. Then it generates an app manifest and stores it in your Steam folder. After restarting Steam, it should appear in your Steam library.

Steam apps folder location vary between different platforms. \
For Windows: `C:\Program Files (x86)\Steam\steamapps`\
For Linux: `/home/{user}/.local/share/Steam/steamapps`
### Generate Multiple App Manifests
~~~
App 1: ...
~~~
Functions like `Generate Single App Manifest` but with multiple app ids. Note that the app ids must be valid and will be skipped if otherwise. This function can be exited by simply pressing `ENTER` on your next app.
>This program supports command-line arguments. When supplied with valid app ids, it will automatically call this function and generate all app manifests into your Steam folder.

>Powershell example: `PS> & '.\Steam App Manifest Fetcher' 3489700 1151640 2420110`\
Bash example: `$ ./"Steam App Manifest Fetcher" 3489700 1151640 2420110`
### Search Applications
~~~
Search app: ...
~~~
This will search the entire database stored locally (if it exists) and return a list of all apps containing the searched name.\
Make sure the name is properly spelled as the function that takes in your search input is case-sensitive.

Note down all of the app ids from the desired apps, pick `Generate Multiple App Manifest` and supply all app ids into that function.
### Update Database
~~~
Downloding database...
~~~
This will download the entire database and stores it locally. It is around 13.5 MB in size.\
Do not update database manually unless you have to.
Such cases include an app not being in the database or that the database itself got corrupted.
>If the database doesn't exist and you choose either `Generate Single App Manifest` or `Generate Multiple App Manifests`, it will automatically download the database and store it locally. Note that you will need to relaunch the program.
## License
This program is licensed under the [MIT License](https://opensource.org/license/mit).
