# table.html
This is the layout of matches. I've used HTML because I intend to let the user modify the table. 
The only script that should look at the content of table.html is HTMLdocument.cs, which performs a find-and-replace on these strings:
* %date%
* %rink%
* %size%
* %delete%
* %lead1%
* %lead2%
* %second1%
* %second2%
* %third1%
* %third2%
* %skip1%
* %skip2%
* %lead1pref%
* %lead2pref%
* %second1pref%
* %second2pref%
* %third1pref%
* %third2pref%
* %skip1pref%
* %skip2pref%
* %leadvisible%
* %secondvisible%
* %thirdvisible%
* %skipvisible%

It also searches for `<!--repeat-->` to avoid duplicating the entire document.
