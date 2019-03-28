the webservice contain the following web methods:
/********************************************************
CreateMatch
GetGuestNum
GetHostNum
GetMatchData
GetOnlineMatches
GetPlayerData
GetTopPlayers
JoinMatch
Login
MatchDataGuestRemUpdate       //update the remainig playing amount
MatchDataHostRemUpdate
MatchDataUpdate
MatchDataWinnerUpdate
PlayerDataUpdate
PlayerLossUpdate
PlayerStarsUpdate
PlayerStatusUpdate
PlayerTotalUpdate
PlayerWinUpdate
Register
RemoveMatchEnd
/********************************************************
i call them by "httpGet" function by passing the parameters in URL form,
and receiving the result in Json form.
the interface call this methods and receive the json result with "android-query" Library.


RandomX
this android app is divide three parts :

1)database "SQL server"
2)Webservice "c#"
3)android interface "java" "android studio"

The idea of this game is simple:
Choose a number of 4 non-duplicate numbers in your mind,as well your competitor.
the first one who guess the other number's, wins.
the app will help you throw, by :

1)Sign-up.
2)sign-in.
3.1)creating an match or join to other from the list of the online matches.
3.2)the amount in this game divided into a total amount and the playing amount decided by the host who created the match.

4)viewing your profile.
5)deciding the winner, and adding the money that the other player losse to it's account, and counting the number of times they lost or won.



/*******************************************************************************
game rules:
1) Must be two players to start the match.

2) every player have a total amount in his account.

3) when starting or joining a match a part of the total amount selected by host is used to play with "the Match price".

**this match price is used to pay for your guesses if it's finished that mean your done and all your "total amount" go to the winner.

4) if a player gues the other number's the amount he earn will be calculated as much as the (amount played with )* 10.

5) if you had played with all your playing money without guessing the other number's 
you loss all your total amount.

at the registration you will get 1000.
every day you get a 500 to your account.

2 ways to win :

a) if you guess the other number's
b) if the other player loss

2 ways to lose:

a)if you play with all your playing money
b)if other player wins

/*******************************************************************************
see video and screenshots for more clarity.
