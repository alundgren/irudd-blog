# Irudd Blog
Combination personal blog and a way to experiment with new tech. May or may not contain actual posts at some point.

Based on the angular template app created using: dotnet new angular.

* Debug using npm: `dotnet run`

Tech used:

* angular
* dotnetcore
* synching to google drive
* clientside login using google
* docker
* markdown

Environment variables:

* NODE_ENV: production | development
* ASPNETCORE_ENVIRONMENT: Production | Development
* IRUDD_BLOG_GOOGLE_USERID: Userid of the the google user that is considered the blog owner (anyone can login but only this user can post)
* IRUDD_BLOG_GOOGLE_CLIENTID: Clientid used for login
* IRUDD_BLOG_GOOGLEDRIVE_CREDENTIALS_FILE: Service account credentials json file used to do serverside synching of posts to google drive

Appsettings:

* GoogleDriveTargetFolder: Name of a folder in the owners personal google drive that is then shared with the service account for synching posts.
* IsGoogleDriveSynchEnabled: true | false. Turns synching to google drive on or off.


The published blog lives [here](https://blog.irudd.se/)