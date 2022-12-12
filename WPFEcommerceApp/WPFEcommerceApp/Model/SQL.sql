use master
go

drop database EcommerceApp
go

create database EcommerceApp 
go

use EcommerceApp
go

create table MUser
(
	Id varchar(8) primary key,
	Role varchar(5) check(Role in ('Admin', 'User', 'Shop')) default 'User',
	Name nvarchar(200) not null  default '' ,
	PhoneNumber varchar(10) not null default '' ,
	Email nvarchar(200) not null  default '',
	Address nvarchar(200) not null default '',
	Gender bit not null default 0,
	DOB smalldatetime,
	Description nvarchar(1000) not null  default '' ,
	StatusUser varchar(10) check(StatusUser in ('Banned', 'NotBanned')) default 'NotBanned',
	StatusShop varchar(10) check(StatusShop in ('NotExist','Banned', 'NotBanned')) default 'NotExist',
	SourceImageAva nvarchar(200),
	SourceImageBackground nvarchar(200)
)
create table FavouriteProduct
(
	IdUser varchar(8) not null,
	IdProduct varchar(8) not null,
	primary key(IdUser, IdProduct)
)
create table ImageProduct
(
	IdProduct varchar(8) not null,
	Source varchar(200) not null,
	primary key(IdProduct, Source)
)
create table Product
(
	Id varchar(8) primary key,
	Name nvarchar(200) not null default '',
	IdCategory varchar(8) not null,
	IdBrand varchar(8) not null,
	IdShop varchar(8) not null, 
	Price bigint not null default 0,
	Sale int not null default 0,
	InStock int not null default 0,
	Sold int not null default 0,
	IsOneSize bit not null default 0,
	IsHadSizeS bit not null default 0, 
	IsHadSizeM bit not null default 0, 
	IsHadSizeL bit not null default 0, 
	IsHadSizeXL bit not null default 0, 
	IsHadSizeXXL bit not null default 0,
	Color nvarchar(200) not null default '',
	Description nvarchar(1000) not null default '',
	DateOfSale smalldatetime,
	Status varchar(10) check(Status in ('Banned', 'NotBanned')) default 'NotBanned'
)
create table Cart                                                                                                                       
(
	IdUser varchar(8) foreign key references MUser(Id),
	IdProduct varchar(8) foreign key references Product(Id),
	Amount int default 0,
	IsChooseSizeS bit default 0,
	IsChooseSizeM bit default 0, 
	IsChooseSizeL bit default 0, 
	IsChooseSizeXL bit default 0,
	IsChooseSizeXXL bit default 0,
	primary key(IdUser, IdProduct)
)
create table Category	
(
	Id varchar(8) primary key,
	Name nvarchar(200) not null unique,
	Status varchar(10) check(Status in ('Banned', 'NotBanned')) default 'NotBanned'
)
create table Brand	
(
	Id varchar(8) primary key,
	Name nvarchar(200) not null unique,
	Status varchar(10) check(Status in ('Banned', 'NotBanned')) default 'NotBanned'
)
create table Advertisement
(
	Id varchar(8) primary key,
	Title nvarchar(200) not null default '',
	SubTitle nvarchar(1000) not null default '',
	Image nvarchar(200) not null
)
create table Notification
(
	Id varchar(8) primary key,
	IdReceiver varchar(8) not null,
	IdSender varchar(8) not null,
	Date smalldatetime not null,
	Content nvarchar(1000) not null default ''
)
create table MOrder
(
	Id varchar(8) primary key,
	IdCustomer varchar(8) not null, 
	IdRating varchar(8) null,
	DateBegin smalldatetime,
	DateEnd smalldatetime,
	OrderTotal Bigint not null default 0,
	Status varchar(20) check (Status in ('Processing', 'Delivering', 'Delivered', 'Cancelled', 'Completed')) default 'Processing'
)
create table OrderInfo
(
	IdOrder varchar(8) not null,
	IdProduct varchar(8) not null,
	ImageProduct varchar(200) null,
	Size varchar(5),
	IdShop varchar(8) not null,
	Count int not null default 0,
	TotalPrice Bigint not null default 0,
	primary key(IdOrder, IdProduct) 
)
create table RatingInfo
(
	Id varchar(8) primary key,
	IdRating varchar(8) not null,
	IdProduct varchar(8) not null,
	Rating int 
)
create table Rating
(
	Id varchar(8) primary key,
	DateRating smalldatetime
)
create table BrandRequest
(
	Id varchar(8) primary key,
	IdShop varchar(8),
	Name nvarchar(200) not null unique,
	Reason nvarchar(1000) default '' not null,
)

create table CategoryRequest
(
	Id varchar(8) primary key,
	IdShop varchar(8),
	Name nvarchar(200) not null unique,
	Reason nvarchar(1000) default '' not null,
)

create table ShopRequest
(
	Id varchar(8) primary key,
	IdUser varchar(8) foreign key references Muser(Id),
	Description nvarchar(1000) default '',
)

alter table BrandRequest
add constraint FK_BrandRequest_Shop foreign key (IdShop) references MUser(Id)

alter table CategoryRequest
add constraint FK_CategoryRequest_Shop foreign key (IdShop) references MUser(Id)

alter table dbo.Product
add constraint FK_Product_Shop foreign key (IdShop) references MUser(Id)

alter table dbo.FavouriteProduct
add constraint FK_FP_MUser foreign key (IdUser) references MUser(Id)

alter table dbo.FavouriteProduct
add constraint FK_FP_Product foreign key (IdProduct) references Product(Id)

alter table dbo.ImageProduct
add constraint FK_IP_Product foreign key (IdProduct) references Product(Id)

alter table dbo.Product
add constraint FK_Product_Category foreign key (IdCategory) references Category(Id)

alter table dbo.Product
add constraint FK_Product_Brand foreign key (IdBrand) references Brand(Id)

alter table dbo.MOrder
add constraint FK_MOrder_MUser foreign key (IdCustomer) references MUser(Id)

alter table dbo.OrderInfo
add constraint FK_OrderInfo_MOrder foreign key (IdOrder) references MOrder(Id)

alter table dbo.MOrder
add constraint FK_MOrder_Rating foreign key (IdRating) references Rating(Id)

alter table dbo.RatingInfo
add constraint FK_RatingInfo_Rating foreign key (IdRating) references Rating(Id)

alter table dbo.RatingInfo
add constraint FK_RatingInfo_Product foreign key (IdProduct) references Product(Id)

alter table dbo.Notification
add constraint FK_Notification_UserReceiver foreign key (IdReceiver) references MUser(Id)

alter table dbo.Notification
add constraint FK_Notification_UserSender foreign key (IdSender) references MUser(Id)

alter table dbo.OrderInfo
add constraint FK_OrderInfo_Product foreign key (IdProduct) references Product(Id)

alter table OrderInfo
add constraint FK_Orderinfor_Shop foreign key (IdShop) references MUser(Id) 


--MockData
insert into MUser values
('user01', 'User', 'Hahaha', '01234569', 
'a@a.aa', 'im here', 0, '6-11-2003', 'Nothing', 'NotBanned', 'NotBanned', null, null)

insert into MUser values
('user02', 'Admin', 'Hahaha', '01234569', 
'a@a.aa', 'im here', 0, '6-11-2003', 'Nothing', 'NotBanned', 'NotBanned', null, null)

insert into Rating values
('rate01',null)

insert into Category values
('cate01', 'Category 01', 'NotBanned')

insert into Brand values
('brand1', 'Brand 01', 'NotBanned')

insert into MOrder values
('order01', 'user01', 'rate01', '12-6-2022', null, 1234, 'Processing')

insert into MOrder values
('order02', 'user01', 'rate01', '12-6-2022', null, 1234, 'Delivered')

insert into Product values
('prod01', 'Product01','cate01', 'brand1', 'user02', 123, 5, 50, 20, 
1, 1, 0, 0, 0, 0, 'Dark Smoke', 'Description 0', '12-13-2022', 'NotBanned')

insert into OrderInfo values
('order01', 'prod01', null, 'XXL', 'user02', 5, 666)

insert into OrderInfo values
('order02', 'prod01', null, 'XXL', 'user02', 5, 666)

select * from MUser
select * from Rating
select * from MOrder
select * from Product
select * from OrderInfo