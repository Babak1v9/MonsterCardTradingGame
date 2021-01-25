create extension if not exists "uuid-ossp";

create table "USER" (
	
	user_id 		uuid primary key default uuid_generate_v4(),
	username		varchar unique not null,
	name			varchar,
	password		varchar not null,
	token			varchar unique not null,
	bio			varchar,
	image			varchar,
	coins			integer default 20,
	games_played		integer	default 0,
	salt			varchar,
	elo			integer,
	wins			integer
);

create table "CARDS" (

	id	 		uuid primary key default uuid_generate_v4(),
	name			varchar not null,
	card_type		varchar not null,
	element_type 		varchar not null,
	damage			varchar not null,
	card_id			varchar,
	pack_id			varchar
);

create table "DECKS" (

	id			uuid primary key default uuid_generate_v4(),
	deck_id			varchar not null,
	user_id 		uuid not null, 
	cardid			varchar not null
);

create table "STACK" (

	stack_id		uuid primary key default uuid_generate_v4(),
	user_id			uuid unique not null,
	
	foreign 		key (USER_ID) references "USER" (USER_ID)
);

create table "PACKAGES" (

	pack_id 		uuid primary key default uuid_generate_v4(),
	price			integer	default 5

);

create table "PACKAGES_CARD" (

	pack_id 		uuid primary key not null,
	card_id 		uuid not null,
);

create table "STORE" (
	
	id			uuid primary key default uuid_generate_v4(),
	trading_id		varchar unique not null,
	user_id 		uuid not null,
	card_id			varchar not null,
	type_requirement	varchar not null,
	element_requirement	integer,
	damage_requirement	integer
);



















