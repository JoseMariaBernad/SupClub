# SupClub
SupClub is an open source application for Xamarin Forms platform.

Its intended use is for the users of any Paddle Surf club to be able to make reservations of their boards.

Known Paddle Surf clubs using this application:
- [Aficionados SUP Zaragoza](http://supclub.es)

## How to Install
1. Clone or [Download](https://github.com/JoseMariaBernad/SupClub/archive/master.zip) this repository.
2. Go to your [Firebase Console](https://console.firebase.google.com/).
3. Create new Firebase project.

![Add project](https://lh3.googleusercontent.com/6NqWWvFG2FCTv9jWKo2zAfSoNlgTWyP9exAc1tTZawVHkSxHlDTr5TzB0CNFMMJJOX4aEkiO1Is "Add project")

4. Under Authentication, Add Email/Password Sign In method.
![Sign-In Method](https://lh3.googleusercontent.com/tQQ_Vltm0rww31nU8Y17xf9sBzyurTkX7HTwcSdt3Q8Vyv9iErfukHVv2rCOtYf9va68Zy91uTY)
5. Under Database / Data, select Import Json to get an initial database with a valid structure with some initial boards. Use file [supclub-db.json](/SupClubLib/supclub-db.json) as initial sample database with some boards.
![enter image description here](https://lh3.googleusercontent.com/AgrHqcbgUICGwep9tRhAtpr2QfuwQ5hxBkuME3rRX8TI6Bxbl8E7IkMYMvq4YqmQGF9AovyqqtI "Import Json") 

6. Under database / rules, paste the contents of the file [rules.json](/SupClubLib/rules.json) and click Publish.
![enter image description here](https://lh3.googleusercontent.com/Z0PsNOas0NWpaRIuil6MBSO3TXA00dVfKfSCrOKjRpuoCDfCZqFc-vcyB9K8P8dJ0hQlgEb88E8 "Database rules")
7. Under Authentication, click WEB SETUP link.
 ![enter image description here](https://lh3.googleusercontent.com/7BCJ1OVLkMtk38UoqeD76Mq80VaL9fV4gpjX9h3xcaZyWnN4_4LqMbFFW8wB9XUQZ0XhqcHEZtQ "Web Setup")
8. Take note of following variables:
    - apiKey: "xxxxyyyyyyzzzzzzzzzzz"
    - databaseURL:  "https://\<your-firebase-project>.firebaseio.com"

10. In Visual Studio, replace all the occurrences of these strings with the ones obtained in the previous step.
 - \<YourFirebaseApiKey> 
 - \<YourFirebaseDatabaseURL>

<!--stackedit_data:
eyJoaXN0b3J5IjpbMTcxNTM2NzE5OF19
-->