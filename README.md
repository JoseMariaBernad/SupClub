# SupClub
SupClub is an open source application for Xamarin Forms platform.

Its intended use is for the users of any Paddle Surf club to be able to make reservations of their boards.

Known Paddle Surf clubs using this application:
- [Aficionados SUP Zaragoza](http://supclub.es)

## How to Install
1. [Clone](x-github-client://openRepo/https://github.com/JoseMariaBernad/SupClub) or [Download](https://github.com/JoseMariaBernad/SupClub/archive/master.zip) this repository.
2. Go to your [Firebase Console](https://console.firebase.google.com/).
3. Create new Firebase project.![Add project](https://lh3.googleusercontent.com/6NqWWvFG2FCTv9jWKo2zAfSoNlgTWyP9exAc1tTZawVHkSxHlDTr5TzB0CNFMMJJOX4aEkiO1Is "Add project")
4. Under Authentication, Add Email/Password Sign In method. ![Sign-In Method](https://lh3.googleusercontent.com/tQQ_Vltm0rww31nU8Y17xf9sBzyurTkX7HTwcSdt3Q8Vyv9iErfukHVv2rCOtYf9va68Zy91uTY)
5. Under database / rules, paste the contents of the file [rules.json](/SupClubLib/rules.json) and click Publish![enter image description here](https://picasaweb.google.com/113767045920483541963/6527921325248096417#6527929061395633922 "Edit rules")
<!--stackedit_data:
eyJoaXN0b3J5IjpbMTkyNjg4Mjc5MF19
-->