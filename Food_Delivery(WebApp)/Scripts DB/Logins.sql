create table UserLogin(
	id int primary key generated always as identity,
	UserName varchar(64) not null,
	passcode varchar(64) not null,
	isActive int default 1
);

insert into UserLogin values
(default, 'admin', 'admin');