# Monster Trading Card Game

This C# console application was created as part of my 3rd Semester of my bachelor degree at FH Technikum Wien.
This HTTP/REST-based server is built to be a platform for trading and battling with and against each other in a magical card-game world.


## Usage 

Execute through IDE or Myserver.exe

## Database

Server uses localhost PostgreSQL DB. Set up with included "create.sql".

The database should listen to localhost (127.0.0.1) on pot 5432.
DB Name = SWE
User = postgres
PW = swe

## Requirements 

* a user is a registered player with credentials (unique username, password).
* a user can manage his cards.
* a card consists of: a name and multiple attributes (damage, element type).
* a card is either a spell card or a monster card.
* a user has multiple cards in his stack.
* a stack is the collection of all his current cards (hint: cards can be removed
* by trading).
* a user can buy cards by acquiring packages.
* a package consists of 5 cards and can be acquired from the server by paying 5 virtual coins.
* every user has 20 coins to buy (4) packages.
* the best 4 cards are selected by the user to be used in the deck.
* the deck is used in the battles against other players.
* a battle is a request to the server to compete against another user with your currently defined deck (see detail description below).

## Setup

Users can
* register and login (token based) to the server,
* acquire some cards,
* define a deck of monsters/spells and
* battle against each other.

Hereby you can trade cards to have better chances to win (see detail description
below). The data should be persisted in a postgreSQL database.

Further features:
* scoreboard / user stats
* editable profile page
* user stats
  * ELO (+3 points for win, -5 for loss, starting value: 100; higher
sophisticated ELO system welcome)

## Security 

A token-based login should be implemented (see: curl script "authorization"-header).
During the registration process (not perfect, but to keep the project simple) a token
is generated which needs to be checked at each call on the server-side (also
identifies the user).
To keep the token usage simple we geneate the token with simply adding a string "-
mtcgToken" to the user name.
User: "altenhof" Token: "{Username}-mtcgToken" (Monster Trading Card Game Token)
HTTP-Header to add on the client: "Authorization: Basic altenhof-mtcgToken"

## Battle Logic

Your cards are split into 2 categories:

* monster cards
  * cards with active attacks and damage based on an element type (fire, water,
  normal). The element type does not effect pure monster fights.
  
* spell cards
  * a spell card can attack with an element based spell (again fire, water, normal) which is either
   * effective (eg: water is effective against fire, so damage is doubled)
   * not effective (eg: fire is not effective against water, so damage is halved)
   * no effect (eg: normal monster vs normal spell, no change of damage, direct comparison between damages)
    
Effectiveness:
* water -> fire
* fire -> normal
* normal -> water

The starting player is chosen randomly. Cards are chosen randomly each round from the
deck to compete (this means 1 round is a battle of 2 cards = 1 of each player). Pure
monster fights are not affected by the element type. As soon as 1 spell cards is
played the element type has an effect on the damage calculation of this single round.
Each round the card with higher calculated damage wins. In case of a draw the
attacking card loses the round. Defeated monsters/spells of the competitor are removed
from the competitor's deck and are taken over in the deck of the current player (vice
versa). Because endless loops are possible we limit the count of rounds to 100.

As a result of the battle we want to return a log which describes the battle in great
detail. Afterwards the stats need to be updated (count of games played and ELO
calculation). Attention: both players must be actively involved (open call to server)
in the game (blocking call) to compete against each other.

The following specialties are to consider:

* Goblins are too afraid of Dragons to attack.
* Wizzard can control Orks so they are not able to damage them.
* The armor of Knights is so heavy that WaterSpells make them drown them instantly.
* The Kraken is immune against spells.
* The FireElves know Dragons since they were little and can evade their attacks.
(Use inheritance to solve this problem)

## Trading Deals
You can request a trading deal by pushing a card into the store (MUST NOT BE IN THE
DECK and is locked for the deck in further usage) and add a requirement for the card
to trade with (eg: "spell or monster" and "min-damage: 50"). Define: Spell or Monster
and additionally a type requirement or a minimum damage.

