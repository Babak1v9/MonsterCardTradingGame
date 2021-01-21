create extension if not exists "uuid-ossp";

create table "USER" (
	
	USER_ID 	uuid primary key default uuid_generate_v4(),
	USERNAME	varchar unique not null,
	NAME		varchar,
	PASSWORD	varchar not null,
	TOKEN 		varchar unique not null,
	BIO		varchar,
	IMAGE		varchar,
	COINS		integer default 20,
	GAMES_PLAYED	integer	default 0
);

create table "CARD" (

	CARD_ID 	uuid primary key default uuid_generate_v4(),
	NAME 		varchar not null,
	CARD_TYPE 	integer not null,
	ELEMENT_TYPE 	integer not null,
	DAMAGE		float not null
);

create table "DECK" (

	DECK_ID		uuid primary key default uuid_generate_v4(),
	USER_ID 	uuid unique not null, 

	foreigny key (USER_ID) references "USER" (USER_ID)
);

create table "DECK_CARD" (

	DECK_ID uuid not null,
	CARD_ID uuid not null,

	primary key (DECK_ID, CARD_ID),
	foreign key (CARD_ID) references "CARD" (CARD_ID),
	foreign key (DECK_ID) references "DECK" (DECK_ID)
);

create table "STACK" (

	STACK_ID uuid primary key default uuid_generate_v4(),
	USER_ID uuid unique not null,
	
	foreign key (USER_ID) references "USER" (USER_ID)
);

create table "STACK_CARD" (
	
	STACK_ID uuid	not null,
	CARD_ID uuid	not null,
	LOCKED boolean not null default false,
	
	primary key (STACK_ID, CARD_ID),
	foreign key (STACK_ID) references "STACK" (STACK_ID),
	foreign key (CARD_ID) references "CARD" (CARD_ID)
);


create table "PACK" (

	PACK_ID uuid primary key default uuid_generate_v4(),
	PRICE integer	default 5
);

create table "PACK_CARD" (

	PACK_ID uuid not null,
	CARD_ID uuid not null,
	
	primary key (PACK_ID, CARD_ID),
	foreign key (PACK_ID) references "PACK" (PACK_ID),
	foreign key (CARD_ID) references "CARD" (CARD_ID)
);



















