create table UserLogin(
	id int primary key generated always as identity,
	UserName varchar(64) not null,
	Passcode varchar(64) not null,
	Status int default 1,
	AdditionalId int
);