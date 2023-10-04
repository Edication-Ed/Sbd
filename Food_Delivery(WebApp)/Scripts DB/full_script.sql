--Удаление таблиц, если есть--------------------------------------------------------------------------------------------------------
DROP TABLE IF EXISTS Dish, Curier, Customer, Client, Orders, DeliveryList, Dish_Order_List;


--Создание таблиц-------------------------------------------------------------------------------------------------------------------
CREATE TABLE Dish --Блюдо
(
	ID_Dish INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
	Dish_Name VARCHAR(80) NOT NULL UNIQUE, 								--Название блюда. Только уникальные значения
	Dish_Cost NUMERIC(10, 2) NOT NULL 									--Цена блюда
);

CREATE TABLE Curier --Доставщик
(
	ID_Curier INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
	Curier_LastName VARCHAR(80) NOT NULL DEFAULT 'Петров', 												--Фамилия
	Curier_FirstName VARCHAR(80) NOT NULL DEFAULT 'Петр', 												--Имя
	Curier_Patronymic VARCHAR(80),																		--Отчество
	Curier_PhoneNumber CHAR(11) NOT NULL, 																--Номер телефона
	Delivery_Type VARCHAR(10) NOT NULL CHECK (Delivery_Type IN ('Пешая', 'Велосипед', 'Автомобиль')), 	--Тип доставки
	Birthday DATE NOT NULL CHECK ((CURRENT_DATE - Birthday) > 18), 										--Дата рождения
	Passport_Series CHAR(4) NOT NULL, 																	--Серия паспорта
	Passport_Number CHAR(6) NOT NULL, 																	--Номер паспорта
	Passport_IssuedBy VARCHAR(255) NOT NULL, 															--Кем выдан
	Passport_Department CHAR(7) NOT NULL 																--Код подразделения
);

CREATE TABLE Client --Клиент
(
	ID_Client INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
	Client_LastName VARCHAR(80) NOT NULL DEFAULT 'Иванов', 				--Фамилия
	Client_FirstName VARCHAR(80) NOT NULL DEFAULT 'Иван', 				--Имя
	Client_Patronymic VARCHAR(80), 										--Отчество
	Client_PhoneNumber CHAR(11) NOT NULL, 								--Номер телефона
	City VARCHAR(50) NOT NULL, 											--Город
	Street VARCHAR(50) NOT NULL, 										--Улица
	House_Number SMALLINT NOT NULL CHECK (House_Number > 0), 			--Номер дома
	Building CHAR(1) 													--Строение/корпус
	--Apartment SMALLINT CHECK (Apartment > 0) 							--Квартира Этот столбец добавится в пунктке с изменением таблиц, который после их создания
);

--Изменение таблицы Клиент----------------------------------------------------------------------------------------------------------
ALTER TABLE Client RENAME TO Customer;													--Переименовывание таблицы

ALTER TABLE Customer RENAME COLUMN ID_Client TO ID_Customer; 							--Переименование столбцов в таблице
ALTER TABLE Customer RENAME COLUMN Client_LastName TO Customer_LastName; 				--Переименование столбцов в таблице
ALTER TABLE Customer RENAME COLUMN Client_FirstName TO Customer_FirstName;				--Переименование столбцов в таблице
ALTER TABLE Customer RENAME COLUMN Client_Patronymic TO Customer_Patronymic;			--Переименование столбцов в таблице
ALTER TABLE Customer RENAME COLUMN Client_PhoneNumber TO Customer_PhoneNumber;			--Переименование столбцов в таблице

ALTER TABLE Customer ADD COLUMN Apartment INT; 											--Добавление столбца

ALTER TABLE Customer ALTER COLUMN Apartment TYPE SMALLINT;								--Смена типа данных у столбца
ALTER TABLE Customer ADD CONSTRAINT check_apartment_bigger_zero CHECK (Apartment > 0); 	--Проверка на 0
ALTER TABLE Customer ALTER COLUMN Apartment SET NOT NULL;								--Смена обязательности у столбца
ALTER TABLE Customer ALTER COLUMN Apartment DROP NOT NULL;								--Смена обязательности у столбца
------------------------------------------------------------------------------------------------------------------------------------

CREATE TABLE Orders --Заказ
(
	ID_Orders INT NOT NULL, --PRIMARY KEY GENERATED ALWAYS AS IDENTITY, Первичный ключ укажется в последующем изменении после таблицы.
	ID_Customer_FK CHAR(5) NOT NULL,
	Time_Ordered TIMESTAMP NOT NULL DEFAULT NOW(), 						--Время заказа. Если не заполнить вручную, то по умолчанию будет поставлены текущее дата и время
	TotalCost NUMERIC(10, 2) NOT NULL CHECK (TotalCost > 0) 			--Общая стоимость
	-- FOREIGN KEY (ID_Customer_FK) REFERENCES Customer(ID_Customer) ON DELETE CASCADE Также укажется в изменении
);
--Изменение таблицы Заказ-----------------------------------------------------------------------------------------------------------
ALTER TABLE Orders ADD COLUMN FK_FOR_Customer INT NOT NULL; 							--Добавление столбца
ALTER TABLE Orders DROP COLUMN FK_FOR_Customer; 										--Удаление столбца

ALTER TABLE Orders ALTER COLUMN ID_Customer_FK TYPE CHAR(10);
--ALTER TABLE Orders ALTER COLUMN ID_Customer_FK TYPE INT; 								--Смена типа данных не сработает, поэтому в комменте
ALTER TABLE Orders ALTER COLUMN ID_Customer_FK TYPE INT USING (ID_Customer_FK::INT);	--Смена типа данных сработает

ALTER TABLE Orders ADD PRIMARY KEY(ID_Orders);											--Добавление первичного ключа к столбцу
ALTER TABLE Orders ALTER COLUMN ID_Orders ADD GENERATED ALWAYS AS IDENTITY;				--Добавление автоинкремента
ALTER TABLE Orders ADD CONSTRAINT fk_id_customer FOREIGN KEY (ID_Customer_FK) REFERENCES Customer(ID_Customer) ON DELETE CASCADE; --Добавление ограничения внешнего ключа к столбцу с каскадным удалением.
------------------------------------------------------------------------------------------------------------------------------------

CREATE TABLE Dish_Order_List --Список блюд
(
	ID_Dish_Order_List INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
	ID_Orders_FK INT NOT NULL, 											--Заказ
	ID_Dish_FK INT NOT NULL, 											--Блюдо в заказе
	Quantity INT NOT NULL CHECK (Quantity > 0), 						--Количество блюд. Должно быть больше 0
	FOREIGN KEY (ID_Orders_FK) REFERENCES Orders(ID_Orders) ON DELETE CASCADE,
	FOREIGN KEY (ID_Dish_FK) REFERENCES Dish(ID_Dish) ON DELETE CASCADE
);

CREATE TABLE DeliveryList --Список доставок
(
	ID_DeliveryList INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
	ID_Orders_FK INT NOT NULL, 																									 	--Доставляемый заказ
	ID_Curier_FK INT NOT NULL, 																									 	--Курьер, который его доставляет
	Time_Delivered TIMESTAMP, 																									 	--Время окончания доставки
	Payment_Type VARCHAR(11) NOT NULL CHECK (Payment_Type IN ('Наличная', 'Безналичная')), 										 	--Тип оплаты
	Delivery_Completion VARCHAR(6) CONSTRAINT delivery_completion_values CHECK (Delivery_Completion IN ('Да', 'Нет', 'Отмена')), 	--Завершение доставки 
	FOREIGN KEY (ID_Orders_FK) REFERENCES Orders(ID_Orders) ON DELETE CASCADE,
	FOREIGN KEY (ID_Curier_FK) REFERENCES Curier(ID_Curier) ON DELETE CASCADE
);


--Заполнение таблиц-----------------------------------------------------------------------------------------------------------------
--Блюдо-----------------------------------------------------------------------------------------------------------------------------
INSERT INTO Dish (Dish_Name, Dish_Cost) 
VALUES ('Панна котта', 199), ('Мисосиру', 199), ('Огуречный салат', 299), ('Жареная цветная капуста', 299),
	   ('KATSU SANDO', 499), ('Удон с говядиной', 499), ('Паста Тобико', 599), ('Паста UNI', 699),
 	   ('DOMOKASE SUSHI', 899), ('DOKKAEBI SUSHI', 999);
											   

--Доставщик--------------------------------------------------------------------------------------------------------------------------
INSERT INTO Curier (Curier_LastName, Curier_FirstName, Curier_Patronymic, Curier_PhoneNumber, Delivery_Type, Birthday, Passport_Series, Passport_Number, Passport_IssuedBy, Passport_Department) 
VALUES ('Синдзив', 'Григорий', 'Геннадьевич', '89201234567', 'Пешая', '2000-12-12', '4877', '480272', 'Отделом внутренних дел России по г. Железногорск', '470-839'),
	   ('Воликов', 'Илья', 'Федотович', '89491977283', 'Велосипед', '2003-05-26', '1316', '141100', 'Отделением УФМС России в г. Коломна', '860-237'),
	   ('Аска', 'Анастасия', 'Емельяновна', '89210123445', 'Автомобиль', '1996-11-11', '4456', '133815', 'Отделением УФМС России в г. Новочебоксарск', '910-332'),
	   ('Мисатова', 'Алиса', 'Николаевна', '89595788271', 'Пешая', '2003-12-08', '4778', '627568', 'Отделением УФМС России по г. Кисловодск', '220-676');

--Клиент-----------------------------------------------------------------------------------------------------------------------------
INSERT INTO Customer (Customer_LastName, Customer_FirstName, Customer_Patronymic, Customer_PhoneNumber, City, Street, House_Number, Building, Apartment) 
VALUES ('Урбан', 'Владислав', 'Савельев', '89255412881', 'Ярославль', 'Новоселов', 24, NULL, 60),
	   ('Ганичев', 'Валентин', 'Владимирович', '89091707447', 'Ярославль', 'Гагарина', 21, 'б', 67),
	   ('Зырянов', 'Кирилл', 'Андреевич', '89187818914', 'Ярославль', 'Космонавтов', 102, NULL, NULL),
	   ('Яшин', 'Андрей', 'Игоревич', '89204748243', 'Ярославль', 'Кузнецова', 3, 'а', NULL),
	   ('Аянами', 'Рей', NULL, '89231921968', 'Ярославль', 'Шоссейная', 9, 'в', 213);
	   
--Заказ------------------------------------------------------------------------------------------------------------------------------
INSERT INTO Orders (ID_Customer_FK, TotalCost) 
VALUES (1, 1594), 				-- Блюда 1-2-3(3)-4
	   (1, 1697), 				-- Блюда 1-5-9
	   (2, 998),				-- Блюда 5-6
	   (2, 1498),				-- Блюда 6-10
	   (3, 1398), 				-- Блюда 7-8
	   (4, 1594), 				-- Блюда 1(4)-6-3
	   (4, 898),  				-- Блюда 3-7
	   (4, 2897); 				-- Блюда 9-10(2)

--Список доставок---------------------------------------------------------------------------------------------------------------------
INSERT INTO DeliveryList (ID_Orders_FK, ID_Curier_FK, Time_Delivered, Payment_Type, Delivery_Completion) 
VALUES (1, 1, '2022-11-20 14:33', 'Безналичная', 'Да'),
	   (2, 3, '2022-11-20 14:11', 'Безналичная', 'Да'),
	   (3, 2, '2022-11-20 14:15', 'Наличная', 'Да'),
	   (4, 1, '2022-11-20 14:29', 'Наличная', 'Да'),
	   (5, 4, '2022-11-20 14:25', 'Безналичная', 'Да'),
	   (6, 2, NULL, 'Наличная', 'Нет'),
	   (7, 3, NULL, 'Наличная', 'Отмена'),
	   (8, 3, '2022-11-20 14:27', 'Безналичная', 'Да');

--Список блюд-------------------------------------------------------------------------------------------------------------------------
INSERT INTO Dish_Order_List (ID_Orders_FK, ID_Dish_FK, Quantity) --Номер заказа, номер блюда, его количество
VALUES (1, 1, 1), (1, 2, 1), (1, 3, 3), (1, 4, 1), 	--1й заказ
	   (2, 1, 1), (2, 5, 1), (2, 9, 1), 			--2й
	   (3, 5, 1), (3, 6, 1), 						--3й
	   (4, 6, 1), (4, 10, 1), 						--4й
	   (5, 7, 1), (5, 8, 1), 						--5й
	   (6, 1, 4), (6, 6, 1), (6, 6, 3), 			--6й
	   (7, 3, 1), (7, 7, 1), 						--7й
	   (8, 9, 1), (8, 10, 2); 						--8й

--SELECT * FROM Orders

--SELECT * FROM Dish_Order_List dol JOIN Orders ord ON ord.ID_Orders = dol.ID_Orders_FK

create table UserLogin(
	id int primary key generated always as identity,
	UserName varchar(64) not null,
	Passcode varchar(64) not null,
	Status int default 1,
	AdditionalId int
);
