use master
go

drop database EcommerceApp
go

create database EcommerceApp 
go

use EcommerceApp
go

--Define Table
create table UserLogin (
	IdUser varchar(30) primary key,
	CreatedDate datetime,
	Password varchar(100),
	Username nvarchar(100) unique,
	Salt varchar(100),
	Provider int, --0: EmailPassword, 1: Google, -1: Admin
)

create table MUser (
	Id varchar(30) primary key,
	Role varchar(5) check(Role in ('Admin', 'User', 'Shop')) default 'User',
	Name nvarchar(200) not null  default '' ,
	PhoneNumber varchar(15) not null default '',
	Email nvarchar(200) not null  default '',
	Gender bit not null default 0,
	DOB smalldatetime,
	Description nvarchar(1000) not null  default '' ,
	StatusUser varchar(10) check(StatusUser in ('Banned', 'NotBanned')) default 'NotBanned',
	StatusShop varchar(10) check(StatusShop in ('NotExist','Banned', 'NotBanned')) default 'NotExist',
	SourceImageAva nvarchar(200),
	SourceImageBackground nvarchar(200),
	DefaultAddress varchar(14)
)

create table Product (
	Id varchar(8) primary key,
	Name nvarchar(200) not null default '',
	IdCategory varchar(8) not null,
	IdBrand varchar(8) not null,
	IdShop varchar(30) not null, 
	Price float(53) not null default 0,
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
	BanLevel int, -- 0: not ban
)

create table FavouriteProduct (
	IdUser varchar(30) not null,
	IdProduct varchar(8) not null,
	primary key(IdUser, IdProduct)
)

create table ImageProduct (
	IdProduct varchar(8) not null,
	Source varchar(200) not null,
	primary key(IdProduct, Source)
)

create table Cart (
	IdUser varchar(30) foreign key references MUser(Id),
	IdProduct varchar(8) foreign key references Product(Id),
	Amount int default 1,
	Size varchar(7) check(Size in ('S', 'M', 'L', 'XL', 'XXL', 'OneSize'))
	primary key(IdUser, IdProduct, Size)
)

create table Category (
	Id varchar(8) primary key,
	Name nvarchar(200) not null unique,
	Status varchar(10) check(Status in ('Banned', 'NotBanned')) default 'NotBanned'
)

create table Brand (
	Id varchar(8) primary key,
	Name nvarchar(200) not null unique,
	Status varchar(10) check(Status in ('Banned', 'NotBanned')) default 'NotBanned'
)

create table Advertisement (
	Id varchar(8) primary key,
	Image nvarchar(200) not null
)

create table AdInUse (
	Id varchar(8) foreign key references Advertisement(Id),
	Position int check(Position in (1, 2, 3)) unique,
	Primary key (Id, Position) 
)

create table Notification (
	Id varchar(8) primary key,
	IdReceiver varchar(30) not null,
	IdSender varchar(30) not null,
	Date smalldatetime not null,
	Content nvarchar(1000) not null default '',
	HaveSeen bit, --1: Seen
)

create table MOrder (
	Id varchar(8) primary key,
	IdCustomer varchar(30) not null, 
	IdShop varchar(30) not null,
	ShipTotal float(53) default 0,
	DateBegin smalldatetime,
	DateEnd smalldatetime,
	OrderTotal float(53) not null default 0,
	Discounted float(53),
	Promo varchar(8),
	ShippingMethod int, 
	ShippingSpeedMethod int,
	AddressIndex varchar(14),
	Status varchar(20) check (Status in ('Processing', 'Delivering', 'Delivered', 'Cancelled', 'Completed')) default 'Processing'
)

create table OrderInfo (
	IdOrder varchar(8) not null,
	IdProduct varchar(8) not null,
	IdRating varchar(8) ,
	ImageProduct varchar(200),
	Size varchar(10),
	Count int not null default 0,
	TotalPrice float(53) not null default 0,
	primary key(IdOrder, IdProduct, Size) 
)

create table Rating (
	Id varchar(8) primary key,
	DateRating smalldatetime,
	Rating int,
	Comment nvarchar(500)
)

create table RatingInfo (
	IdUser varchar(30) not null,
	IdRating varchar(8) not null,
	Comment nvarchar(500),
	DateReply datetime not null,
	primary key (IdUser, IdRating, DateReply)
)

create table BrandRequest (
	Id varchar(8) primary key,
	IdShop varchar(30),
	Name nvarchar(200) not null unique,
	Reason nvarchar(1000) default '' not null,
)

create table CategoryRequest (
	Id varchar(8) primary key,
	IdShop varchar(30),
	Name nvarchar(200) not null unique,
	Reason nvarchar(1000) default '' not null,
)

create table ShopRequest(
	Id varchar(8) primary key,
	IdUser varchar(30) foreign key references Muser(Id),
	Description nvarchar(1000) default '',
)

create table Promo(
	Id varchar(8) primary key,
	IdShop varchar(30) not null,
	Code nvarchar(15) not null,
	Name nvarchar(150),
	Description nvarchar(200),
	DateBegin smalldatetime,
	DateEnd smalldatetime,
	Amount int,
	AmountUsed int,
	MaxSale float(53),
	MinCost float(53),
	CustomerType int,
	Sale float(53),
	Status int
)

create table PromoDetail(
	IdProduct varchar(8),
	IdPromo varchar(8),
	primary key (IdProduct, IdPromo)
)

create table Address (
	Id varchar(14), -- yyyy + mm + dd + hh + mm + ss
	IdUser varchar(30),
	Name nvarchar(200),
	PhoneNumber varchar(15),
	Address nvarchar(200),
	Status bit check (Status in (0,1)), -- 0 la bi xoa, 1 la van con
	primary key (Id, IdUser)
)

--Constraint
alter table MOrder
add constraint FK_MOrder_Promo foreign key (Promo) references Promo(Id)

alter table Address
add constraint FK_Address_IdUser foreign key (IdUser) references MUser(Id)

alter table MUser
add constraint FK_MUser_Id foreign key (Id) references UserLogin(IdUser)

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

alter table dbo.Notification
add constraint FK_Notification_UserReceiver foreign key (IdReceiver) references MUser(Id)

alter table dbo.Notification
add constraint FK_Notification_UserSender foreign key (IdSender) references MUser(Id)

alter table dbo.OrderInfo
add constraint FK_OrderInfo_Product foreign key (IdProduct) references Product(Id)

alter table MOrder
add constraint FK_MOrder_Shop foreign key (IdShop) references MUser(Id)

alter table dbo.OrderInfo
add constraint FK_MOrder_Rating foreign key (IdRating) references Rating(Id)

alter table Promo
add constraint FK_Promo_idShop foreign key (IdShop) references MUser(Id)

alter table PromoDetail
add constraint FK_PromoDetail_idProduct foreign key (IdProduct) references Product(Id)

alter table PromoDetail
add constraint FK_PromoDetail_idPromo foreign key (IdPromo) references Promo(Id)

alter table RatingInfo
add constraint FK_RatingInfo_IdUser foreign key (IdUser) references MUser(Id)

alter table RatingInfo
add constraint FK_RatingInfo_IdRating foreign key (IdRating) references Rating(Id)

--MockData
INSERT [dbo].[UserLogin] VALUES (N'admin', '2023-1-16', N'lqZi7R6xGddaRnT/gy0B0pUaZQNBCILNNj0UTCQK6Rk=', N'admin', N'JP3nrSaeTwKVChHa2OhA6XB8VOYbfTcfBDt8lxQzdNw=', 0)
GO
INSERT [dbo].[UserLogin] VALUES (N'shop50', '2023-1-16', N'DV7GZ5+fth9vlhVEVHYXnejHgzhjd58O8DNyOFbRfHo=', N'yauboi@protonmail.com', N'34DpGL/DIHIxxdxh0bgZ0sDFxsN27vh9S8V0MhcoNjQ=', 0)
GO
INSERT [dbo].[UserLogin] VALUES (N'shop51', '2023-1-16', N'uMS3oY91AlY830gSI2xQ/vQ6s8B966Pc77xg7ax1w+w=', N'hwangvu@protonmail.com', N'lvxt4ggH/USzQIgaN+6Yg+8CnaFiT9sM/NEyd5EEHW4=', 0)
GO
INSERT [dbo].[MUser] ([Id], [Role], [Name], [PhoneNumber], [Email], [Gender], [DOB], [Description], [StatusUser], [StatusShop], [SourceImageAva], [SourceImageBackground], [DefaultAddress]) VALUES (N'admin', N'Admin', N'An Vũ', N'0123456789', N'admin', 0, NULL, N'', N'NotBanned', N'NotExist', NULL, NULL, N'20230115235046')
GO
INSERT [dbo].[MUser] ([Id], [Role], [Name], [PhoneNumber], [Email], [Gender], [DOB], [Description], [StatusUser], [StatusShop], [SourceImageAva], [SourceImageBackground], [DefaultAddress]) VALUES (N'shop50', N'Shop', N'The COOL Shop', N'0123456789', N'yauboi@protonmail.com', 0, NULL, N'- Chuyên nhập các loại quần áo từ các thương hiệu nổi tiếng và nhận Order. 
- Giao hàng và hỗ trợ đổi trả trong tuần đầu tiên.
- Có bảo hành và hóa đơn đầy đủ.
- Cam kết chất lượng.', N'NotBanned', N'NotBanned', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/User%2Fshop50%2FAva?alt=media&token=b2f9ad73-9deb-4abc-839e-9029db9aee40', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/User%2Fshop50%2FBackground?alt=media&token=56f3adaa-b73d-42fb-9997-874067c4aea8', N'20230115234749')
GO
INSERT [dbo].[MUser] ([Id], [Role], [Name], [PhoneNumber], [Email], [Gender], [DOB], [Description], [StatusUser], [StatusShop], [SourceImageAva], [SourceImageBackground], [DefaultAddress]) VALUES (N'shop51', N'Shop', N'Small .G Shop', N'0123456789', N'hwangvu@protonmail.com', 0, NULL, N'- Chuyên nhập các loại quần áo từ các thương hiệu nổi tiếng và nhận Order. 
- Giao hàng và hỗ trợ đổi trả trong tuần đầu tiên.
- Có bảo hành và hóa đơn đầy đủ.
- Cam kết chất lượng.', N'NotBanned', N'NotBanned', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/User%2Fshop51%2FAva?alt=media&token=1cd14761-dc7e-4569-9a4a-5fbe7d385540', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/User%2Fshop51%2FBackground?alt=media&token=437bbde3-6b94-402b-aa7c-78ee58dcf1c6', N'20230115235733')
GO
INSERT [dbo].[BrandRequest] ([Id], [IdShop], [Name], [Reason]) VALUES (N'breq50', N'shop50', N'Jazz', N'Jazz is a music genre that originated in the African-American communities of New Orleans, Louisiana, in the late 19th and early 20th centuries, with its roots in blues and ragtime.')
GO
INSERT [dbo].[BrandRequest] ([Id], [IdShop], [Name], [Reason]) VALUES (N'breq51', N'shop51', N'City pop', N'City pop (シティ・ポップ, shiti poppu) is a loosely defined form of Japanese pop music that emerged in the late 1970s and peaked in the 1980s, however the term "City Pop" would first be used in the 90s')
GO
INSERT [dbo].[BrandRequest] ([Id], [IdShop], [Name], [Reason]) VALUES (N'breq52', N'shop50', N'Rock', N'Rock music is a broad genre of popular music that originated as "rock and roll" in the United States in the late 1940s and early 1950s, developing into a range of different styles in the mid-1960s and later, particularly in the United States and United Kingdom.')
GO
INSERT [dbo].[BrandRequest] ([Id], [IdShop], [Name], [Reason]) VALUES (N'breq53', N'shop51', N'Blues', N'Blues is a music genre and musical form which originated in the Deep South of the United States around the 1860s')
GO
INSERT [dbo].[Category] ([Id], [Name], [Status]) VALUES (N'cat50', N'Coat', N'NotBanned')
GO
INSERT [dbo].[Category] ([Id], [Name], [Status]) VALUES (N'cat51', N'Suit', N'NotBanned')
GO
INSERT [dbo].[Category] ([Id], [Name], [Status]) VALUES (N'cat52', N'Blazer', N'NotBanned')
GO
INSERT [dbo].[Brand] ([Id], [Name], [Status]) VALUES (N'brand50', N'Ralph Lauren', N'NotBanned')
GO
INSERT [dbo].[Brand] ([Id], [Name], [Status]) VALUES (N'brand51', N'DOCUMENT', N'NotBanned')
GO
INSERT [dbo].[Brand] ([Id], [Name], [Status]) VALUES (N'brand52', N'TOTEME', N'NotBanned')
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod50', N'Kent Pinstripe Cotton-Wool Twill Suit', N'cat51', N'brand50', N'shop50', 2995, 30, 232, 1, 0, 0, 1, 0, 1, 0, N'Black/Cream', N'Tailored in Italy, the Kent combines a lighter construction with fine canvassing and a soft shoulder to create a more natural profile. This version, which is paired with our pleated Gregory trousers, is crafted from a lightweight custom-developed twill.', CAST(N'2023-01-17T06:54:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod51', N'AUSTRIA FELT WOOL BALMACAAN COAT', N'cat50', N'brand51', N'shop51', 1026, 0, 36, 0, 0, 1, 1, 1, 0, 0, N'Dark Brown', N'도큐먼트의  발마칸 코트가 열여섯 번째 시즌에는  오스트리아에서 생산된 펠트 울로 짜여진 원단으로 출시 되었습니다. 

클래식한 아이템을 도큐먼트의 언어로 해석하는 THE DOCUMENT 시리즈로써,  요셉보이스의 펠트 수트의 영감을 받은 펠트 울로 발마칸 코트를 선보입니다.

소프트하고 가볍지만, 튼튼한 펠트 울의 오묘한 카키컬러가 상당히 매력적입니다.

요셉보이스 작가의 상징적인 펠트 직물로 만들어져 기념하여 요셉보이스 라벨이 함께 부착되어 있습니다.

/

내부에 충전재를 추가하여, 입체감있는 실루엣을 드러내며, 보온성을 높여 줍니다.

안감은 탄성이 좋고, 따뜻한 질감의 안감을 사용하며, 전체적인 질감의 균형을 유지하며, 마찰이 많은 소매단 안쪽은  면직물을 사용해 내구성을 보완하며,

캐주얼하게 걷어 입었을 경우에 다른 매력를 선사합니다. 

클래식한 발마칸 코트와는 다른 도큐먼트만의 벨트 여밈이 들어가 있어서, 탈 착이 가능하며, 앞에서 나올 수 있게 되어 있는 디테일이 뒷 라인의 실루엣을 유지하며,

벨트 여밈을 할 수 있게 합니다. 

앞 여밈의 아래쪽에는 바람에 코트가 올라 가지 않도록 채울 수 있는 비조 장식이 있습니다. 

라글란 소매, 풍성한 실루엣과 긴 기장감이 특징입니다.', CAST(N'2023-01-17T05:51:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod52', N'Signature shearling coat black/off-white', N'cat50', N'brand52', N'shop50', 3900, 0, 16, 1, 0, 1, 1, 1, 0, 0, N'Black/off-white', N'Signature TOTEME coat made from heavyweight leather arranged into symmetrical panels. It has an oversized shape showing glimpses of the warm off-white shearling interior at the shawl lapels and cuffs, then is finished with a silver zipper and front in-seam pockets. Layer it over your favorite knits.', CAST(N'2023-01-17T07:07:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod53', N'Signature shearling coat macadamia', N'cat50', N'brand52', N'shop51', 3900, 0, 120, 0, 0, 1, 1, 1, 1, 1, N'Macadamia', N'Signature TOTEME coat made from heavyweight suede arranged into symmetrical panels. It has an oversized shape trimmed at the shawl lapels and cuffs with off-white shearling as a preview of the warm interior, then is finished with a silver zipper and front in-seam pockets. Layer it over your favorite knits.', CAST(N'2023-01-17T06:06:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod54', N'BROWN CHECK JACKET', N'cat52', N'brand51', N'shop51', 433, 10, 123, 4, 0, 1, 1, 1, 1, 0, N'Brown', N'이탈리아 LANIFICIO 통해서 개발한 린넨 비스코스 원단으로 제작된 BROWN CHECK JACKET 입니다.

vintage 한 분위기가 드러나는 외관에서부터 유니크함과 고급스러움이 드러나는 독특한 모습을 보여줍니다.

EARTH 컬러의 깊은 컬러감이 오히려 시원한 느낌을 주며,  자연스럽게 wrinkled 되어있는 표면이 특징입니다.

두번째 단추부터 여미도록 롤링 라펠로 되어있으며, 라펠까지 닫을 수 있도록 설계되어 있습니다.

여유있는 루즈한 핏으로 제 사이즈 구매를 추천드립니다. 

BROWN CHECK 시리즈의 3-pieces 셋업으로도 즐기실 수 있습니다.', CAST(N'2023-01-17T06:30:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod55', N'Kent Handmade Tartan Jacket', N'cat51', N'brand50', N'shop50', 4995, 30, 13, 0, 0, 0, 1, 1, 1, 0, N'Green/Navy/Black', N'Hand-tailored in Italy, the Kent combines a lighter construction with fine canvassing and a soft shoulder to create a more natural profile. This version is crafted from an authentic Scottish Black Watch tartan fabric that was custom-developed for Ralph Lauren using fine wool and cashmere.', CAST(N'2023-01-17T06:46:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod56', N'Burnham Water-Repellent Trench Coat', N'cat50', N'brand50', N'shop50', 1895, 30, 116, 1, 0, 1, 1, 0, 0, 0, N'Polo Black', N'Purple Label elevates the classic trench coat with expert Italian craftsmanship, medium-weight cotton twill, and supple leather details.', CAST(N'2023-01-17T07:03:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod57', N'Kent Handmade Cashmere Blazer', N'cat52', N'brand50', N'shop51', 5495, 0, 5, 0, 0, 0, 0, 0, 1, 0, N'Royal Navy', N'Hand-tailored in Italy, the Kent combines a lighter construction with fine canvassing and a soft shoulder to create a more natural profile. This version''s cashmere twill was woven exclusively for Ralph Lauren at England''s storied Bower Roebuck mill. To complete this impeccable piece, silver-tone-brass buttons are engraved with our signature crest.', CAST(N'2023-01-17T06:17:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod58', N'Kent Chalk-Stripe Wool Flannel Suit', N'cat51', N'brand50', N'shop50', 3695, 30, 136, 0, 0, 0, 1, 0, 0, 1, N'Brown/Cream', N'Crafted in Italy, the Kent combines a lighter construction with fine canvassing and a soft shoulder to create a more natural profile. This version is tailored from wool flannel that was custom-developed for Ralph Lauren Purple Label with our signature matte finish.', CAST(N'2023-01-17T06:51:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod59', N'Polo Soft Stretch Chino Suit Jacket', N'cat51', N'brand50', N'shop51', 398, 0, 100, 0, 0, 1, 1, 1, 1, 0, N'Tan', N'The Polo Soft fit puts a modern spin on classic preppy style with a natural shoulder and a half-canvassed construction. This version is tailored from stretch cotton twill and garment-dyed to create a relaxed look.', CAST(N'2023-01-16T00:45:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod60', N'Bouclé wool peak lapel coat black', N'cat50', N'brand52', N'shop51', 1800, 0, 35, 1, 1, 0, 0, 0, 0, 0, N'Black', N'TOTEME coat tailored to a smart, slim silhouette made of textured bouclé containing wool and alpaca fibers. It has padded shoulders, peak lapels and welt pockets, and fastens with a concealed button placket that opens to reveal a satin lining. Contrast the tactile material with a shirt and jeans.', CAST(N'2023-01-17T06:02:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod61', N'Polo Soft Glen Plaid Wool Suit', N'cat51', N'brand50', N'shop51', 1498, 0, 131, 2, 0, 0, 1, 1, 1, 1, N'Black/Cream/Blue', N'The Polo Soft fit puts a modern spin on classic preppy style with a natural shoulder and a half-canvassed construction. This version is tailored in Italy from an English wool fabric.
- Jacket: peak lapels; buttonhole at the left lapel; double-breasted silhouette; cuffs with four decorative buttons.
- Jacket: left chest welt pocket; two front waist flapped pockets; two interior chest pockets; double vent; half-canvassed; lined at the back yoke and the sleeves.
- Trouser: side adjusters; zip fly with an extended buttoned closure; curtain waistband; side on-seam pockets; right hip ticket pocket; two back buttoned pockets; lined to the knees at the front.
- Trouser comes unhemmed with a 38" inseam. Alterations available in Ralph Lauren retail stores.
- 100% wool. Dry clean. Made in Italy.', CAST(N'2023-01-17T06:24:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod62', N'THE DOCUMENT BALMACAAN COAT_BEIGE', N'cat50', N'brand51', N'shop50', 734, 0, 30, 0, 0, 1, 1, 1, 0, 0, N'Rose White', N'긴 기장감과 풍성한 실루엣, 라글란 소매가 특징인 BALMACAAN COAT가 12TH REPETITION AND DIFFERENCE 로 선보이게 됩니다.

5oz 패드를 충전재로 하여 입체적인 실루엣을 하며 기존 THE DOCUMENT BALAMACAAN COAT 스타일을 고수합니다.

메인으로 사용된 원단은 다양한 소재의 혼방 코튼 소재가 평직 조직으로 짜여져 수직으로 드러나있는 다채로운 컬러감이 보여지는 표면과 풍성한 터치감이 돋보이고 내구성이 뛰어납니다.

길게 뻗은 뒷트임은 활동성을 보장하며 트임을 열고 닫을 수 있도록 비조 장식이 되어 있습니다.

제원단으로 되어있는 벨트 여밈이 있으며 벨트를 여밈에 따라 클래식한 발마칸 코트와는 다른 뉘앙스를 보이기도 합니다.

소매 안쪽은 면트윌 원단을 사용하여 내구성을 보완하고 충전재를 감싸는 스트레치 안감은 편안함을 제공합니다.

착용자를 배려하여 설계된 만큼 다양한 장점을 가지고 있습니다.

직접 입어보시는 것을 권장 드리며 100사이즈 기준 M 입으시면 좋습니다.', CAST(N'2023-01-17T07:05:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod63', N'DOUBLE FACE COTTON RAGLAN JACKET _ YELLOW CHECK', N'cat52', N'brand51', N'shop50', 372, 0, 123, 0, 0, 1, 1, 1, 1, 0, N'Yellow Check', N'일본 코지마 지역에서 생산되는 쇼와 텍스사의 양면이 다른 원단으로 제작되었습니다.  

홑겹으로 이루어져 있어서 여름에도 입도록, 가볍고 착용감이 좋습니다.

소매는 안감이 없이 홑겹으로 되어 있지만, 입고 벗을 때 불편함이 없도록 여유로운 실루엣을 이루며, 걷어 입을수 있도록 속으로 겉감 원단과 동일하도록 제작되었습니다.

라펠을 들어 올려서 채워 입을 수 있는 것은 물론 3 버튼으로 보이지만, 2 버튼으로 채워지도록 설계되어진 롤링 라펠을 이루고 있습니다. 

관련 제품( 반바지와 셔츠 ) 셋업으로 즐기실 수 있습니다.', CAST(N'2023-01-17T07:16:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod64', N'ENGLISH WOOL RAGLAN JACKET', N'cat52', N'brand51', N'shop50', 469, 0, 321, 0, 0, 1, 1, 1, 1, 0, N'Fiord', N'오랜 전통을 가지고 있는 영국의 MOON 사의 원단으로 만들어진 래글런 셋업 쟈켓입니다.

적당히 여유있는 핏으로 잘 테일러드 된 울 쟈켓의 느낌을 보여줍니다.

앞 여밈이 교차되는 반 더블 여밈으로 버튼을 잠그지 않고 있었을 때 자연스러운 실루엣이 돋보이고, 

큐프라 안감을 사용하여 착용감을 보완하였습니다.

가벼우며, 부드럽게 몸을 감싸는 느낌이 질 좋은 쟈켓을 느끼게 해 줍니다.', CAST(N'2023-01-17T07:18:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod65', N'Shearling Polo Coat', N'cat50', N'brand50', N'shop50', 4295, 30, 26, 0, 0, 0, 0, 1, 0, 0, N'Snuff', N'The Polo coat is named for its popularity among English polo players who would warm up in the garment between matches. Ralph Lauren''s shearling version maintains iconic details, such as the double-breasted silhouette and belted back, while elevating it with expert Italian craftsmanship.', CAST(N'2023-01-17T07:01:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod66', N'Belted evening blazer black', N'cat52', N'brand52', N'shop51', 920, 0, 3, 1, 0, 0, 0, 1, 1, 1, N'Black', N'TOTEME evening blazer tailored to a longline silhouette that''s cinched at the waist with a matching, detachable belt. The slightly lustrous fabric is woven from cotton, wool and polyamide yarns and additional details include padded shoulders, welt pockets, a back vent, and satin lining. Complete the suit with the Sewn pleat evening trousers.', CAST(N'2023-01-17T06:13:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod67', N'HEAVY MELTON COAT_NAVY', N'cat50', N'brand51', N'shop51', 773, 0, 136, 0, 0, 0, 1, 1, 1, 0, N'Navy', N'지난 시즌 처음으로 선보인 HEAVY MELTON COAT 가 지난해 사랑에 힘입어 반복되어 출시 되었습니다. 

28 온스의  울 멜톤 원단으로 제작되어 옷 한 벌에서 느껴지는 강직함이 있지만,  캐시미어와 라이크라가 혼합되어 부드럽고, 몸에 감기는 듯한 느낌을 받으실 수 있습니다. 

안사양의 떠있는 듯한 구조와 누빔지와 옥스포드로 이루어진 구성은 documentation 라인 특유의 모습을 하고 있으며 documentation 라벨이 부착되어 있습니다.

라글란 소매에 인체 공학적인 휜 소매 패턴은 착장시 편안함을 이끌어내며, 몸판의 다트는 인체 구조와 닮도록 디자인 되었습니다.

소매는 두 장 소매에서 안 소매가 분할되어 총 세장으로 되어 있으며 테일러링 됐음을 느낄 수 있습니다.

앞을 여미는 플라켓에서부터 내려오는 마감은 밑단 마감까지 경쾌한 듯 곡선적인 실루엣을 드러내며, 착장시 걸을 때 유려한 선을 이끌어냅니다. 

넉넉한 수납이 가능한 옆주머니 외에도 가슴 전면부의 포켓은 밑이 둥글게 그려져 있는 형태로 코트 전체의 뉘앙스를 나타냅니다.

무게감 있는 DOCUMENT의 뉘앙스를 겪으시길 원하시는 분은 documentation 라인의 코트를 직접 느껴보시기 바랍니다.', CAST(N'2023-01-17T05:55:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod68', N'Polo Wool Sharkskin 3-Piece Suit', N'cat51', N'brand50', N'shop51', 1598, 0, 230, 0, 0, 0, 0, 1, 1, 1, N'Light Brown', N'Ralph Lauren''s double-breasted Polo suit marries impeccable Italian craftsmanship with a wool sharkskin fabric from England. Rolled lapels and hand-sewn natural shoulders distinguish the half-canvassed jacket, which pairs with double-pleated trousers.', CAST(N'2023-01-17T05:35:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod69', N'Longline wool blazer brown check', N'cat52', N'brand52', N'shop50', 980, 0, 231, 0, 0, 1, 1, 1, 1, 1, N'Brown check', N'TOTEME blazer reimagined in a mid-weight houndstooth/checked wool blend. It has a relaxed silhouette with a single-breasted button placket, shoulder pads, welt pockets and black satin lining. Complement the elongated shape with straight trousers.', CAST(N'2023-01-17T07:15:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod70', N'Kent Handmade Glen Plaid Linen-Wool Suit', N'cat51', N'brand50', N'shop50', 4199, 0, 5, 0, 0, 0, 0, 1, 0, 0, N'Cream/Black/Purple', N'Hand-tailored in Italy, the Kent combines a lighter construction with fine canvassing and a soft shoulder to create a more natural profile. This version and its Gregory trousers are crafted from a linen-and-wool fabric that was custom-developed for Ralph Lauren.', CAST(N'2023-01-17T06:42:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod71', N'Wool bouclé cape black', N'cat52', N'brand52', N'shop50', 590, 0, 123, 0, 0, 1, 1, 1, 0, 0, N'Black', N'TOTEME evening cape constructed to a minimalist A-line silhouette that dips gracefully at the back. It''s made from tactile bouclé that contains wool and alpaca fibers and is finished with a frayed-edged hem, a hook-and-eye fastening, and satin lining. Style it with the Panelled leather dress or the Sewn pleat evening trousers.', CAST(N'2023-01-17T07:13:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod72', N'THE DOCUMENT DUFFLE COAT', N'cat50', N'brand51', N'shop51', 817, 0, 9, 0, 0, 1, 1, 1, 0, 0, N'Dark Navy', N'8번째 컬렉션에 첫 공개 이후, 지난 겨울 시즌마다 리스톡되어 14번째 반복과차이에서도 리스톡되었습니다.

/

도큐먼트에서 더플코트가 나온다면 어떤 느낌일까 상상했습니다.

더플 코트의 고유성을 유지하며, 도큐먼트의 디테일이 녹아들어 도큐먼트 더플 코트를 선보입니다.

이탈리아에서 제작된 원단을 사용하여 제작되었으며, 더플코트의 고유의 버튼과 긴 기장감 여유있는 핏,

그리고, 핸드 워머 포켓이 있습니다. 화이트와 아이보리 톤의 컬러 배색이 매력이며, 허리를 조일 수 있느 스트링과 내피와 

후드가 탈 부착되는 방식입니다. 

내피와 후드 안감으로 누빔 면 원단이 사용되어 보온성을 높여 줍니다.

여유있는 오버 사이즈 패턴으로 설계되어서, 안쪽에 레이어드 해서 보온과 스타일을 추가 할 수 있습니다.

내피는 바깥의 주머니와 단독으로 채워서 입을 수 있도록 제작되어서 실용성을 높여 줍니다. 

지퍼로도 채울 수 있으며,  원형 단추로도 채울수 있게 , 혹은 리얼 뿔 단추로 채울 수 있게 되어 있어서 다양한 연출이 가능합니다.

특수 가공된 노끈 사용으로 끈에서 나오는 먼지가 없도록 하며, YKK 알루미늄 지퍼를 사용하여, 시각적인 쾌감을 선사합니다.', CAST(N'2023-01-17T06:10:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod73', N'Suede Balmacaan Coat', N'cat50', N'brand50', N'shop51', 6995, 0, 9, 0, 0, 0, 0, 1, 1, 0, N'Santa Barbara Brown', N'Purple Label''s elevated take on the classic balmacaan is crafted in Italy from supple, custom-dyed suede and finished with genuine horn buttons.
- Point collar. Buttoned throat latch. Buttoned placket. Genuine horn buttons.
- Long sleeves. Adjustable buttoned tabs at the cuffs.
- Two front waist buttoned pockets.
- Belt loops. Self-belt with a suede-covered buckle. Single vent. Fully lined.
- Shell: suede. Body lining: cotton, viscose. Sleeve lining: viscose.
- Dry clean by a leather specialist.
- Made in Italy.', CAST(N'2023-01-17T06:20:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Promo] ([Id], [IdShop], [Code], [Name], [Description], [DateBegin], [DateEnd], [Amount], [AmountUsed], [MaxSale], [MinCost], [CustomerType], [Sale], [Status]) VALUES (N'promo50', N'shop51', N'NICEDAY', N'Have a nice day', N'Have a nice day!', CAST(N'2023-01-11T12:00:00' AS SmallDateTime), CAST(N'2023-03-31T12:00:00' AS SmallDateTime), -1, 0, 150, 100, 0, 20, 1)
GO
INSERT [dbo].[Promo] ([Id], [IdShop], [Code], [Name], [Description], [DateBegin], [DateEnd], [Amount], [AmountUsed], [MaxSale], [MinCost], [CustomerType], [Sale], [Status]) VALUES (N'promo51', N'shop51', N'NEWYEAR2023', N'HAPPY NEW YEAR', N'Happy new year 2023.', CAST(N'2023-01-15T12:00:00' AS SmallDateTime), CAST(N'2023-03-31T12:00:00' AS SmallDateTime), -1, 0, 300, 300, 0, 30, 1)
GO
INSERT [dbo].[Promo] ([Id], [IdShop], [Code], [Name], [Description], [DateBegin], [DateEnd], [Amount], [AmountUsed], [MaxSale], [MinCost], [CustomerType], [Sale], [Status]) VALUES (N'promo52', N'shop51', N'BIGBIG', N'ALMOST FREE', N'BIG SALE BIG SALE', CAST(N'2023-01-18T12:00:00' AS SmallDateTime), CAST(N'2023-03-28T12:00:00' AS SmallDateTime), -1, 0, 1.7976931348623157E+308, 0, 0, 99, 0)
GO
INSERT [dbo].[Promo] ([Id], [IdShop], [Code], [Name], [Description], [DateBegin], [DateEnd], [Amount], [AmountUsed], [MaxSale], [MinCost], [CustomerType], [Sale], [Status]) VALUES (N'promo53', N'shop50', N'COOLBIGSALE', N'BIG SALEE', N'Sale 100%', CAST(N'2023-02-02T12:00:00' AS SmallDateTime), CAST(N'2023-02-28T12:00:00' AS SmallDateTime), -1, 0, 1.7976931348623157E+308, 0, 0, 100, 0)
GO
INSERT [dbo].[Promo] ([Id], [IdShop], [Code], [Name], [Description], [DateBegin], [DateEnd], [Amount], [AmountUsed], [MaxSale], [MinCost], [CustomerType], [Sale], [Status]) VALUES (N'promo54', N'shop50', N'NEWUSER123', N'WELCOME TO WANO', N'Welcome to WANO.
This Promo is for you.
Enjoy it!!!', CAST(N'2023-01-11T12:00:00' AS SmallDateTime), CAST(N'2023-02-20T12:00:00' AS SmallDateTime), -1, 0, 100, 0, 0, 30, 1)
GO
INSERT [dbo].[Promo] ([Id], [IdShop], [Code], [Name], [Description], [DateBegin], [DateEnd], [Amount], [AmountUsed], [MaxSale], [MinCost], [CustomerType], [Sale], [Status]) VALUES (N'promo55', N'shop50', N'NEWYEAR2023', N'HAPPY NEW YEAR', N'Happy new year Everyone.
This Promo is for you.
Enjoy it!', CAST(N'2023-01-18T12:00:00' AS SmallDateTime), CAST(N'2023-03-31T12:00:00' AS SmallDateTime), -1, 0, 300, 300, 0, 30, 2)
GO
INSERT [dbo].[Promo] ([Id], [IdShop], [Code], [Name], [Description], [DateBegin], [DateEnd], [Amount], [AmountUsed], [MaxSale], [MinCost], [CustomerType], [Sale], [Status]) VALUES (N'promo56', N'shop50', N'NEWYEAR2023', N'HAPPY NEW YEAR', N'Happy new year Everyone
This Promo is for you.
Enjoy it!', CAST(N'2023-01-18T12:00:00' AS SmallDateTime), CAST(N'2023-03-31T12:00:00' AS SmallDateTime), -1, 0, 300, 300, 0, 30, 1)
GO
INSERT [dbo].[MOrder] ([Id], [IdCustomer], [IdShop], [ShipTotal], [DateBegin], [DateEnd], [OrderTotal], [Discounted], [Promo], [ShippingMethod], [ShippingSpeedMethod], [AddressIndex], [Status]) VALUES (N'MOrd50', N'shop50', N'shop51', 0, CAST(N'2023-01-17T08:54:00' AS SmallDateTime), CAST(N'2023-01-17T08:57:00' AS SmallDateTime), 5219, 43.3, N'promo51', 1, 1, N'20230115234749', N'Completed')
GO
INSERT [dbo].[MOrder] ([Id], [IdCustomer], [IdShop], [ShipTotal], [DateBegin], [DateEnd], [OrderTotal], [Discounted], [Promo], [ShippingMethod], [ShippingSpeedMethod], [AddressIndex], [Status]) VALUES (N'MOrd51', N'shop50', N'shop51', 0, CAST(N'2023-01-17T08:54:00' AS SmallDateTime), NULL, 2371, 0, N'promo50', 1, 0, N'20230115234749', N'Delivering')
GO
INSERT [dbo].[MOrder] ([Id], [IdCustomer], [IdShop], [ShipTotal], [DateBegin], [DateEnd], [OrderTotal], [Discounted], [Promo], [ShippingMethod], [ShippingSpeedMethod], [AddressIndex], [Status]) VALUES (N'MOrd52', N'shop50', N'shop51', 0, CAST(N'2023-01-17T08:54:00' AS SmallDateTime), NULL, 398, 0, NULL, 1, 0, N'20230117085452', N'Delivering')
GO
INSERT [dbo].[MOrder] ([Id], [IdCustomer], [IdShop], [ShipTotal], [DateBegin], [DateEnd], [OrderTotal], [Discounted], [Promo], [ShippingMethod], [ShippingSpeedMethod], [AddressIndex], [Status]) VALUES (N'MOrd53', N'shop50', N'shop51', 0, CAST(N'2023-01-17T08:55:00' AS SmallDateTime), CAST(N'2023-01-17T08:57:00' AS SmallDateTime), 2089, 129.9, N'promo51', 2, 1, N'20230117085452', N'Delivered')
GO
INSERT [dbo].[MOrder] ([Id], [IdCustomer], [IdShop], [ShipTotal], [DateBegin], [DateEnd], [OrderTotal], [Discounted], [Promo], [ShippingMethod], [ShippingSpeedMethod], [AddressIndex], [Status]) VALUES (N'MOrd54', N'shop51', N'shop50', 0, CAST(N'2023-01-17T08:58:00' AS SmallDateTime), NULL, 1570, 0, N'promo54', 2, 1, N'20230115235733', N'Delivering')
GO
INSERT [dbo].[MOrder] ([Id], [IdCustomer], [IdShop], [ShipTotal], [DateBegin], [DateEnd], [OrderTotal], [Discounted], [Promo], [ShippingMethod], [ShippingSpeedMethod], [AddressIndex], [Status]) VALUES (N'MOrd55', N'shop51', N'shop50', 0, CAST(N'2023-01-17T09:00:00' AS SmallDateTime), CAST(N'2023-01-17T09:08:00' AS SmallDateTime), 6865, 898.5, N'promo56', 1, 1, N'20230115235733', N'Completed')
GO
INSERT [dbo].[MOrder] ([Id], [IdCustomer], [IdShop], [ShipTotal], [DateBegin], [DateEnd], [OrderTotal], [Discounted], [Promo], [ShippingMethod], [ShippingSpeedMethod], [AddressIndex], [Status]) VALUES (N'MOrd56', N'shop51', N'shop50', 0, CAST(N'2023-01-17T09:00:00' AS SmallDateTime), CAST(N'2023-01-17T09:09:00' AS SmallDateTime), 1865, 568.5, NULL, 1, 0, N'20230115235733', N'Completed')
GO
INSERT [dbo].[MOrder] ([Id], [IdCustomer], [IdShop], [ShipTotal], [DateBegin], [DateEnd], [OrderTotal], [Discounted], [Promo], [ShippingMethod], [ShippingSpeedMethod], [AddressIndex], [Status]) VALUES (N'MOrd57', N'shop51', N'shop50', 0, CAST(N'2023-01-17T09:00:00' AS SmallDateTime), NULL, 5137, 0, N'promo54', NULL, 1, N'20230115235733', N'Processing')
GO
INSERT [dbo].[Notification] ([Id], [IdReceiver], [IdSender], [Date], [Content], [HaveSeen]) VALUES (N'noti50', N'shop50', N'admin', CAST(N'2023-01-16T00:35:00' AS SmallDateTime), N'Your brand request has been accepted.', 1)
GO
INSERT [dbo].[Notification] ([Id], [IdReceiver], [IdSender], [Date], [Content], [HaveSeen]) VALUES (N'noti51', N'shop50', N'admin', CAST(N'2023-01-17T07:32:00' AS SmallDateTime), N'Your promo request is accepted.', 1)
GO
INSERT [dbo].[Notification] ([Id], [IdReceiver], [IdSender], [Date], [Content], [HaveSeen]) VALUES (N'noti52', N'shop51', N'admin', CAST(N'2023-01-16T00:03:00' AS SmallDateTime), N'Your request to be a shop has been accepted. Check out your shop and start selling.', 1)
GO
INSERT [dbo].[Notification] ([Id], [IdReceiver], [IdSender], [Date], [Content], [HaveSeen]) VALUES (N'noti53', N'shop50', N'admin', CAST(N'2023-01-17T07:32:00' AS SmallDateTime), N'Your promo request is accepted.', 1)
GO
INSERT [dbo].[Notification] ([Id], [IdReceiver], [IdSender], [Date], [Content], [HaveSeen]) VALUES (N'noti54', N'shop50', N'admin', CAST(N'2023-01-16T00:35:00' AS SmallDateTime), N'Your brand request has been accepted.', 1)
GO
INSERT [dbo].[Notification] ([Id], [IdReceiver], [IdSender], [Date], [Content], [HaveSeen]) VALUES (N'noti55', N'shop51', N'admin', CAST(N'2023-01-16T00:37:00' AS SmallDateTime), N'Your brand request has been accepted.', 1)
GO
INSERT [dbo].[Notification] ([Id], [IdReceiver], [IdSender], [Date], [Content], [HaveSeen]) VALUES (N'noti56', N'shop50', N'admin', CAST(N'2023-01-16T00:03:00' AS SmallDateTime), N'Your request to be a shop has been accepted. Check out your shop and start selling.', 1)
GO
INSERT [dbo].[Notification] ([Id], [IdReceiver], [IdSender], [Date], [Content], [HaveSeen]) VALUES (N'noti57', N'shop50', N'admin', CAST(N'2023-01-16T00:35:00' AS SmallDateTime), N'Your category request has been accepted.', 1)
GO
INSERT [dbo].[Notification] ([Id], [IdReceiver], [IdSender], [Date], [Content], [HaveSeen]) VALUES (N'noti58', N'shop50', N'admin', CAST(N'2023-01-16T00:35:00' AS SmallDateTime), N'Your category request has been accepted.', 1)
GO
INSERT [dbo].[Notification] ([Id], [IdReceiver], [IdSender], [Date], [Content], [HaveSeen]) VALUES (N'noti59', N'shop50', N'admin', CAST(N'2023-01-16T00:35:00' AS SmallDateTime), N'Your category request has been accepted.', 1)
GO
INSERT [dbo].[Notification] ([Id], [IdReceiver], [IdSender], [Date], [Content], [HaveSeen]) VALUES (N'noti60', N'shop51', N'admin', CAST(N'2023-01-17T08:10:00' AS SmallDateTime), N'Your promo request is accepted.', 1)
GO
INSERT [dbo].[Notification] ([Id], [IdReceiver], [IdSender], [Date], [Content], [HaveSeen]) VALUES (N'noti61', N'shop51', N'admin', CAST(N'2023-01-17T08:10:00' AS SmallDateTime), N'Your promo request is accepted.', 1)
GO
INSERT [dbo].[Notification] ([Id], [IdReceiver], [IdSender], [Date], [Content], [HaveSeen]) VALUES (N'Noti62', N'shop50', N'shop51', CAST(N'2023-01-17T08:57:00' AS SmallDateTime), N'The order MOrd53 is delivering.', 1)
GO
INSERT [dbo].[Notification] ([Id], [IdReceiver], [IdSender], [Date], [Content], [HaveSeen]) VALUES (N'Noti63', N'shop50', N'shop51', CAST(N'2023-01-17T08:57:00' AS SmallDateTime), N'The order MOrd53 is delivered. Please check and notify us if something is wrong.', 1)
GO
INSERT [dbo].[Notification] ([Id], [IdReceiver], [IdSender], [Date], [Content], [HaveSeen]) VALUES (N'Noti64', N'shop50', N'shop51', CAST(N'2023-01-17T08:57:00' AS SmallDateTime), N'The order MOrd50 is delivering.', 1)
GO
INSERT [dbo].[Notification] ([Id], [IdReceiver], [IdSender], [Date], [Content], [HaveSeen]) VALUES (N'Noti65', N'shop50', N'shop51', CAST(N'2023-01-17T08:57:00' AS SmallDateTime), N'The order MOrd50 is delivered. Please check and notify us if something is wrong.', 1)
GO
INSERT [dbo].[Notification] ([Id], [IdReceiver], [IdSender], [Date], [Content], [HaveSeen]) VALUES (N'Noti66', N'shop50', N'shop51', CAST(N'2023-01-17T08:57:00' AS SmallDateTime), N'The order MOrd52 is delivering.', 1)
GO
INSERT [dbo].[Notification] ([Id], [IdReceiver], [IdSender], [Date], [Content], [HaveSeen]) VALUES (N'Noti67', N'shop50', N'shop51', CAST(N'2023-01-17T09:06:00' AS SmallDateTime), N'The order MOrd51 is delivering.', 1)
GO
INSERT [dbo].[Notification] ([Id], [IdReceiver], [IdSender], [Date], [Content], [HaveSeen]) VALUES (N'Noti68', N'shop51', N'shop50', CAST(N'2023-01-17T09:08:00' AS SmallDateTime), N'The order MOrd55 is delivering.', 1)
GO
INSERT [dbo].[Notification] ([Id], [IdReceiver], [IdSender], [Date], [Content], [HaveSeen]) VALUES (N'Noti69', N'shop51', N'shop50', CAST(N'2023-01-17T09:08:00' AS SmallDateTime), N'The order MOrd56 is delivering.', 1)
GO
INSERT [dbo].[Notification] ([Id], [IdReceiver], [IdSender], [Date], [Content], [HaveSeen]) VALUES (N'Noti70', N'shop51', N'shop50', CAST(N'2023-01-17T09:08:00' AS SmallDateTime), N'The order MOrd54 is delivering.', 1)
GO
INSERT [dbo].[Notification] ([Id], [IdReceiver], [IdSender], [Date], [Content], [HaveSeen]) VALUES (N'Noti71', N'shop51', N'shop50', CAST(N'2023-01-17T09:08:00' AS SmallDateTime), N'The order MOrd55 is delivered. Please check and notify us if something is wrong.', 1)
GO
INSERT [dbo].[Notification] ([Id], [IdReceiver], [IdSender], [Date], [Content], [HaveSeen]) VALUES (N'Noti72', N'shop51', N'shop50', CAST(N'2023-01-17T09:09:00' AS SmallDateTime), N'The order MOrd56 is delivered. Please check and notify us if something is wrong.', 1)
GO
INSERT [dbo].[Rating] ([Id], [DateRating], [Rating], [Comment]) VALUES (N'Rati50', CAST(N'2023-01-17T09:04:00' AS SmallDateTime), 5, N'Awesome!!!
I like it a lot.')
GO
INSERT [dbo].[Rating] ([Id], [DateRating], [Rating], [Comment]) VALUES (N'Rati51', CAST(N'2023-01-17T09:04:00' AS SmallDateTime), 5, N'Giao hàng siêu nhanh!!!!')
GO
INSERT [dbo].[Rating] ([Id], [DateRating], [Rating], [Comment]) VALUES (N'Rati52', CAST(N'2023-01-17T09:04:00' AS SmallDateTime), 5, N'Tuyệt vời, tôi dã có 2 nguời yêu sau khi mặc nó dạo 1 vòng quanh phố cổ.')
GO
INSERT [dbo].[Rating] ([Id], [DateRating], [Rating], [Comment]) VALUES (N'Rati53', CAST(N'2023-01-17T09:10:00' AS SmallDateTime), 1, N'Oh dear God, this coat is so nice.
But my girlfriend dumped me today when I wore this coat, so sorry.')
GO
INSERT [dbo].[Rating] ([Id], [DateRating], [Rating], [Comment]) VALUES (N'Rati54', CAST(N'2023-01-17T09:12:00' AS SmallDateTime), 4, NULL)
GO
INSERT [dbo].[Rating] ([Id], [DateRating], [Rating], [Comment]) VALUES (N'Rati55', CAST(N'2023-01-17T09:12:00' AS SmallDateTime), 5, N'Omg my girlfriend came back with me after I gave her this coat.
So take my five stars. Hehehe')
GO
INSERT [dbo].[RatingInfo] ([IdUser], [IdRating], [Comment], [DateReply]) VALUES (N'shop51', N'Rati50', N'Thank you so much.', CAST(N'2023-01-17T09:07:34.740' AS DateTime))
GO
INSERT [dbo].[RatingInfo] ([IdUser], [IdRating], [Comment], [DateReply]) VALUES (N'shop51', N'Rati51', N'My pleasure, thank you.', CAST(N'2023-01-17T09:07:25.243' AS DateTime))
GO
INSERT [dbo].[RatingInfo] ([IdUser], [IdRating], [Comment], [DateReply]) VALUES (N'shop51', N'Rati52', N'That''s so funny, thank you.', CAST(N'2023-01-17T09:06:51.837' AS DateTime))
GO
INSERT [dbo].[Cart] ([IdUser], [IdProduct], [Amount], [Size]) VALUES (N'shop50', N'prod57', 1, N'XL')
GO
INSERT [dbo].[Cart] ([IdUser], [IdProduct], [Amount], [Size]) VALUES (N'shop50', N'prod59', 1, N'S')
GO
INSERT [dbo].[Cart] ([IdUser], [IdProduct], [Amount], [Size]) VALUES (N'shop50', N'prod60', 1, N'OneSize')
GO
INSERT [dbo].[Cart] ([IdUser], [IdProduct], [Amount], [Size]) VALUES (N'shop50', N'prod72', 2, N'S')
GO
INSERT [dbo].[Cart] ([IdUser], [IdProduct], [Amount], [Size]) VALUES (N'shop50', N'prod73', 1, N'L')
GO
INSERT [dbo].[Cart] ([IdUser], [IdProduct], [Amount], [Size]) VALUES (N'shop51', N'prod55', 1, N'M')
GO
INSERT [dbo].[Cart] ([IdUser], [IdProduct], [Amount], [Size]) VALUES (N'shop51', N'prod58', 1, N'M')
GO
INSERT [dbo].[Cart] ([IdUser], [IdProduct], [Amount], [Size]) VALUES (N'shop51', N'prod62', 1, N'S')
GO
INSERT [dbo].[Cart] ([IdUser], [IdProduct], [Amount], [Size]) VALUES (N'shop51', N'prod63', 2, N'S')
GO
INSERT [dbo].[Address] ([Id], [IdUser], [Name], [PhoneNumber], [Address], [Status]) VALUES (N'20230115234749', N'shop50', N'The COOL Shop', N'0123456789', N'936 Kha Vạn Cân, Trường Thọ, Thủ Đức, TP. Hồ Chí Minh', 1)
GO
INSERT [dbo].[Address] ([Id], [IdUser], [Name], [PhoneNumber], [Address], [Status]) VALUES (N'20230115235046', N'admin', N'An Vũ', N'0123456789', N'Đường Hàn Thuyên, khu phố 6 P, Thủ Đức, Thành phố Hồ Chí Minh', 1)
GO
INSERT [dbo].[Address] ([Id], [IdUser], [Name], [PhoneNumber], [Address], [Status]) VALUES (N'20230115235733', N'shop51', N'Small .G Shop', N'0123456789', N'384 Nam Kỳ Khởi Nghĩa, Phường 8, Quận 3, Hồ Chí Minh', 1)
GO
INSERT [dbo].[Address] ([Id], [IdUser], [Name], [PhoneNumber], [Address], [Status]) VALUES (N'20230117085452', N'shop50', N'Jaclyn Pham', N'0912833291', N'27A Quan Hoa, Cầu Giấy, Hà Nội', 1)
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod50', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F0%23%23cZd6b?alt=media&token=033d1475-dd56-4c69-83d4-b633ab14afb8')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod50', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F0%23%23cZd6b_14?alt=media&token=f925cb96-e1ff-4391-b97d-e0f77084dcbb')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod50', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F0%23%23cZd6b_158?alt=media&token=4bf20143-8759-4eb4-bb74-81c3ce296592')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod50', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F0%23%23cZd6b_196?alt=media&token=3d015ed2-4b1c-43d8-a40d-b2f97c0c0cc8')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod50', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F0%23%23cZd6b_64?alt=media&token=74bc6023-10c2-499d-864f-f5ad2dcf8314')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod51', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F0%23%23HZd6c?alt=media&token=40596def-99f0-4a86-bf2c-c366f85aa105')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod51', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F0%23%23HZd6c_174?alt=media&token=50771608-3164-42a9-82c6-0e541bdee34c')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod51', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F0%23%23HZd6c_20?alt=media&token=b7567918-be7e-4571-a154-e548aa0b5868')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod52', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod52%2Fprd8-1.jpg?alt=media&token=5797d3c1-77d0-433c-bdc7-e4b2eaa8fdcc')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod52', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod52%2Fprd8-2.jpg?alt=media&token=960e9d49-1970-454c-b471-a3e4b2d8b7a6')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod52', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod52%2Fprd8-3.jpg?alt=media&token=7d0b1716-13a9-4dcc-b26b-66f019e91fa5')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod52', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod52%2Fprd8-4.jpg?alt=media&token=865f47cf-bbf0-4c2b-92aa-86681b25033f')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod53', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprd6-1.jpg?alt=media&token=db67d65c-3168-4add-ac6b-ea4c73cf0a6e')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod53', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F1%23%23MZd6d_112?alt=media&token=f152be84-a13e-45de-925e-33978f11d46e')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod53', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F1%23%23MZd6d_172?alt=media&token=e81cb948-e3dd-475b-a7e9-cce4dff3a388')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod53', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F1%23%23MZd6d_26?alt=media&token=e77dcc88-d3b4-4671-8df1-90ebe39edec5')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod53', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F1%23%23MZd6d_92?alt=media&token=826d9f3a-a741-4b7b-9bb0-3e1491479c09')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod54', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F1%23%23VZd6c?alt=media&token=a22cff11-e4a2-492d-ba9c-d8fdee3346ae')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod54', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F1%23%23VZd6c_136?alt=media&token=88a7c50b-cb8a-4184-8fe2-5af578c33202')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod54', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F1%23%23VZd6c_68?alt=media&token=007abe5b-2ddd-44c9-8431-e24a04cc6972')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod54', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F1%23%23VZd6c_98?alt=media&token=5c71db14-bd2c-410a-bd9d-c37dc2902d00')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod55', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F1%23%23YZd6f?alt=media&token=a9cf43a5-8b30-4bd6-92b9-c930ef0bd42d')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod55', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F1%23%23YZd6f_192?alt=media&token=ad2cc53c-62f6-466c-9c83-2773ee8fb19f')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod55', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F1%23%23YZd6f_54?alt=media&token=6d64c864-1a68-4c0b-95f9-a954253e4a03')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod55', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F1%23%23YZd6f_84?alt=media&token=dec9ea22-4606-4acd-8fde-eb18d15de0f9')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod56', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F3%23%23fZd6i?alt=media&token=4e3cade8-92e6-4c3a-94c2-27b3cd272522')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod56', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F3%23%23fZd6i_142?alt=media&token=97a7ea47-7fc8-4f98-ac2c-3c97dce27590')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod56', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F3%23%23fZd6i_164?alt=media&token=d3fe81a2-1a97-4762-8406-cf321ff384ab')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod56', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F3%23%23fZd6i_48?alt=media&token=51f05cb7-e0ab-404b-bb4e-a4da04adacdc')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod57', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F3%23%23QZd6k?alt=media&token=1642c2f8-5e17-4f1e-9796-8f524ce41e84')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod57', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F3%23%23QZd6k_154?alt=media&token=3793fa12-6de9-49d1-a5d0-3dd15cd679a0')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod57', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F3%23%23QZd6k_194?alt=media&token=6e834d56-a56a-420c-a754-a55bcdf51d66')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod57', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F3%23%23QZd6k_74?alt=media&token=f5c16215-301e-4972-86f3-e89d3731843c')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod58', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F4%23%23aZd6l?alt=media&token=cd511a77-f7c3-4a9b-9606-37dcac06da26')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod58', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F4%23%23aZd6l_110?alt=media&token=7c4f6bb7-3f70-4df7-a1b3-b1973002ce76')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod58', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F4%23%23aZd6l_148?alt=media&token=5c95f271-ad76-4c83-ad0a-51e67395bffb')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod58', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F4%23%23aZd6l_180?alt=media&token=43c2f412-b1f2-43f1-9cbf-56a6d0d22290')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod58', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F4%23%23aZd6l_90?alt=media&token=e642c132-92ac-4ecd-82f7-0dc3504dee96')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod59', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F4%23%23DZd6k?alt=media&token=8b30da85-d782-4094-b10d-39cdceb33897')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod59', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F4%23%23DZd6k_12?alt=media&token=b5ee30d0-dab0-47db-a3a1-c8720eaeaeaa')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod59', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F4%23%23DZd6k_46?alt=media&token=9df6f186-e4ba-4e3b-9cdf-ce972119561e')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod59', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F4%23%23DZd6k_50?alt=media&token=822f8080-f077-4658-8edf-e1acc84c7f82')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod60', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F4%23%23KZd6k?alt=media&token=1c62bb95-15e0-4e64-84cb-34ba2bee9be2')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod60', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F4%23%23KZd6k_162?alt=media&token=fc188d77-2a79-4017-867d-c02e7177acfb')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod60', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F4%23%23KZd6k_48?alt=media&token=f3c5ba02-1d66-42e9-869f-26f97a21db0e')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod61', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F4%23%23UZd6m?alt=media&token=50de8fe4-8e37-4c47-a6fe-48544681cd1d')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod61', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F4%23%23UZd6m_128?alt=media&token=d5542ac5-9dcf-4067-a8ba-8b85e7a597cb')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod61', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F4%23%23UZd6m_138?alt=media&token=04b81ec9-13f5-404c-a3b6-151498ce3a27')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod61', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F4%23%23UZd6m_46?alt=media&token=97948dba-395b-488a-97bf-4e696219ae85')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod62', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F5%23%23hZd6n?alt=media&token=da2f5007-fc37-48c4-bc93-f89bc3446863')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod62', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F5%23%23hZd6n_52?alt=media&token=454d2a78-3529-4bd9-b800-07e26976dd12')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod62', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F5%23%23hZd6n_66?alt=media&token=2522517a-d896-4983-add9-aba334c52faf')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod63', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F5%23%23nZd6o_0?alt=media&token=f176b4ec-122b-4164-84f5-025340ee5810')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod63', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F5%23%23nZd6o_194?alt=media&token=a66dc279-c430-4cc8-8745-79314d9940d4')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod63', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F5%23%23nZd6o_90?alt=media&token=d1558587-4fdb-4eed-a729-1668a0086589')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod64', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F5%23%23pZd6o?alt=media&token=f2ab5815-dcd0-4cf5-a04e-2e10c9fa1736')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod64', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F5%23%23pZd6o_18?alt=media&token=14a352da-f756-4ab9-858c-3d318ad03381')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod64', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F5%23%23pZd6o_46?alt=media&token=dd4977ad-d2f7-46ba-80d1-6a83aed2afb4')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod65', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F6%23%23dZd6q?alt=media&token=04a39e8e-5046-4428-8df8-9ead90911266')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod65', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F6%23%23dZd6q_136?alt=media&token=404d5caf-e480-4975-9d04-560fdc787601')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod65', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F6%23%23dZd6q_168?alt=media&token=1a6f2c68-2ed1-474f-b807-b4ab6e3b9ddb')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod65', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F6%23%23dZd6q_66?alt=media&token=8a32d8bb-168f-4339-8592-6ccedafccbf7')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod66', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F6%23%23PZd6r?alt=media&token=7720d3ff-4f54-4fcf-8ed1-f5d39e1173ee')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod66', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F6%23%23PZd6r_152?alt=media&token=b0db2a8a-df94-4c2d-915e-7bdf059833a7')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod66', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F6%23%23PZd6r_182?alt=media&token=9467fb76-3c9b-495c-a1ce-40f24a58c688')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod66', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F6%23%23PZd6r_54?alt=media&token=d3385337-45af-489a-9912-db4cdc6f573d')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod67', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F7%23%23IZd6t?alt=media&token=484ec74c-8af2-44cd-969d-8cb49b3dcc8d')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod67', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F7%23%23IZd6t_148?alt=media&token=ad8a3c07-35f9-4411-8d9f-da5d4feb5454')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod67', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F7%23%23IZd6t_160?alt=media&token=d4dfe328-81be-4c6d-b8e5-7dc0cb9a014d')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod67', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F7%23%23IZd6t_98?alt=media&token=ddf8ca44-7bc2-4164-b299-56f7680019f3')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod68', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F8%23%23FZd6w?alt=media&token=30f487e5-4729-4323-85e6-45b18f56fd85')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod68', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F8%23%23FZd6w_40?alt=media&token=9d9675d9-e797-48fc-84b0-d4d0611ac72d')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod68', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F8%23%23FZd6w_44?alt=media&token=4d4996a8-efb4-4d8c-b4e2-b27ac5afa0fc')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod68', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F8%23%23FZd6w_88?alt=media&token=4183861b-1866-4613-a042-0412aeea9ec1')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod69', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F8%23%23lZd6x?alt=media&token=02929835-c7e0-4b33-aa64-113353c96940')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod69', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F8%23%23lZd6x_162?alt=media&token=fdedbe9e-a63c-4e00-bf44-1e03876cf062')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod69', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F8%23%23lZd6x_168?alt=media&token=b07e3ca6-c807-4305-abdc-d432fa9f1535')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod69', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F8%23%23lZd6x_52?alt=media&token=eeb529b7-7c92-4597-9c43-cfef13de6c30')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod70', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F8%23%23XZd6v?alt=media&token=4bdb7f76-0680-4246-819f-5121462bb165')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod70', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F8%23%23XZd6v_14?alt=media&token=f522fd6e-d10d-4dd5-a6ba-273b1501959b')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod70', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F8%23%23XZd6v_22?alt=media&token=0d307c7d-b2ee-4abe-946f-ea1d39724921')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod70', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F8%23%23XZd6v_6?alt=media&token=fdeafb44-91bb-4e84-bea8-8025e2280798')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod71', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F9%23%23kZd6z?alt=media&token=2b169070-3ec8-42d3-9ae0-95175bb853b7')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod71', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F9%23%23kZd6z_188?alt=media&token=2b3c6032-59bf-44c6-9b44-0685a92bfbcc')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod71', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F9%23%23kZd6z_192?alt=media&token=cde6c44d-290b-4870-852e-854c98e98d05')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod71', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F9%23%23kZd6z_2?alt=media&token=bac72f3b-f880-4b2f-a845-1dc7e92e3b47')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod72', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F9%23%23NZd6z?alt=media&token=10b4baed-b9cb-44e2-b68f-2bb1347212ad')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod72', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F9%23%23NZd6z_188?alt=media&token=8e974ec8-e8c8-4fdf-9e21-6ef59368cb58')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod72', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F9%23%23NZd6z_192?alt=media&token=04c3b287-37b8-4dcb-b092-75359e3ad0a6')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod73', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F9%23%23SZd6z?alt=media&token=f72332a2-53db-41f3-8a35-224b88936f9a')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod73', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F9%23%23SZd6z_148?alt=media&token=a0099245-26e4-43de-bfd1-002aba3c1a95')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod73', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F9%23%23SZd6z_156?alt=media&token=354291c3-1362-46f3-812a-fd1da0399c04')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod73', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F9%23%23SZd6z_68?alt=media&token=d02612d0-ab79-466f-960c-93d149c1331a')
GO
INSERT [dbo].[OrderInfo] ([IdOrder], [IdProduct], [IdRating], [ImageProduct], [Size], [Count], [TotalPrice]) VALUES (N'MOrd50', N'prod54', N'Rati50', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F1%23%23VZd6c?alt=media&token=a22cff11-e4a2-492d-ba9c-d8fdee3346ae', N'M', 1, 5185)
GO
INSERT [dbo].[OrderInfo] ([IdOrder], [IdProduct], [IdRating], [ImageProduct], [Size], [Count], [TotalPrice]) VALUES (N'MOrd50', N'prod60', N'Rati51', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F4%23%23KZd6k?alt=media&token=1c62bb95-15e0-4e64-84cb-34ba2bee9be2', N'OneSize', 1, 5185)
GO
INSERT [dbo].[OrderInfo] ([IdOrder], [IdProduct], [IdRating], [ImageProduct], [Size], [Count], [TotalPrice]) VALUES (N'MOrd50', N'prod61', N'Rati52', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F4%23%23UZd6m?alt=media&token=50de8fe4-8e37-4c47-a6fe-48544681cd1d', N'M', 2, 5185)
GO
INSERT [dbo].[OrderInfo] ([IdOrder], [IdProduct], [IdRating], [ImageProduct], [Size], [Count], [TotalPrice]) VALUES (N'MOrd51', N'prod67', NULL, N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F7%23%23IZd6t?alt=media&token=484ec74c-8af2-44cd-969d-8cb49b3dcc8d', N'M', 1, 2371)
GO
INSERT [dbo].[OrderInfo] ([IdOrder], [IdProduct], [IdRating], [ImageProduct], [Size], [Count], [TotalPrice]) VALUES (N'MOrd51', N'prod68', NULL, N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F8%23%23FZd6w?alt=media&token=30f487e5-4729-4323-85e6-45b18f56fd85', N'XL', 1, 2371)
GO
INSERT [dbo].[OrderInfo] ([IdOrder], [IdProduct], [IdRating], [ImageProduct], [Size], [Count], [TotalPrice]) VALUES (N'MOrd52', N'prod59', NULL, N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F4%23%23DZd6k?alt=media&token=8b30da85-d782-4094-b10d-39cdceb33897', N'S', 1, 398)
GO
INSERT [dbo].[OrderInfo] ([IdOrder], [IdProduct], [IdRating], [ImageProduct], [Size], [Count], [TotalPrice]) VALUES (N'MOrd53', N'prod54', NULL, N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F1%23%23VZd6c?alt=media&token=a22cff11-e4a2-492d-ba9c-d8fdee3346ae', N'L', 3, 2089)
GO
INSERT [dbo].[OrderInfo] ([IdOrder], [IdProduct], [IdRating], [ImageProduct], [Size], [Count], [TotalPrice]) VALUES (N'MOrd53', N'prod66', NULL, N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F6%23%23PZd6r?alt=media&token=7720d3ff-4f54-4fcf-8ed1-f5d39e1173ee', N'L', 1, 2089)
GO
INSERT [dbo].[OrderInfo] ([IdOrder], [IdProduct], [IdRating], [ImageProduct], [Size], [Count], [TotalPrice]) VALUES (N'MOrd54', N'prod69', NULL, N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F8%23%23lZd6x?alt=media&token=02929835-c7e0-4b33-aa64-113353c96940', N'S', 1, 1570)
GO
INSERT [dbo].[OrderInfo] ([IdOrder], [IdProduct], [IdRating], [ImageProduct], [Size], [Count], [TotalPrice]) VALUES (N'MOrd54', N'prod71', NULL, N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F9%23%23kZd6z?alt=media&token=2b169070-3ec8-42d3-9ae0-95175bb853b7', N'S', 1, 1570)
GO
INSERT [dbo].[OrderInfo] ([IdOrder], [IdProduct], [IdRating], [ImageProduct], [Size], [Count], [TotalPrice]) VALUES (N'MOrd55', N'prod50', N'Rati54', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F0%23%23cZd6b?alt=media&token=033d1475-dd56-4c69-83d4-b633ab14afb8', N'M', 1, 5996)
GO
INSERT [dbo].[OrderInfo] ([IdOrder], [IdProduct], [IdRating], [ImageProduct], [Size], [Count], [TotalPrice]) VALUES (N'MOrd55', N'prod52', N'Rati55', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F1%23%23iZd6c?alt=media&token=bd90913b-ca8a-454d-9bd8-fd28f0943a1c', N'S', 1, 5996)
GO
INSERT [dbo].[OrderInfo] ([IdOrder], [IdProduct], [IdRating], [ImageProduct], [Size], [Count], [TotalPrice]) VALUES (N'MOrd56', N'prod56', N'Rati53', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F3%23%23fZd6i?alt=media&token=4e3cade8-92e6-4c3a-94c2-27b3cd272522', N'S', 1, 1326)
GO
INSERT [dbo].[OrderInfo] ([IdOrder], [IdProduct], [IdRating], [ImageProduct], [Size], [Count], [TotalPrice]) VALUES (N'MOrd57', N'prod64', NULL, N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F5%23%23pZd6o?alt=media&token=f2ab5815-dcd0-4cf5-a04e-2e10c9fa1736', N'S', 2, 5137)
GO
INSERT [dbo].[OrderInfo] ([IdOrder], [IdProduct], [IdRating], [ImageProduct], [Size], [Count], [TotalPrice]) VALUES (N'MOrd57', N'prod70', NULL, N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F8%23%23XZd6v?alt=media&token=4bdb7f76-0680-4246-819f-5121462bb165', N'L', 1, 5137)
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod50', N'promo53')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod50', N'promo54')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod50', N'promo55')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod50', N'promo56')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod51', N'promo51')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod51', N'promo52')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod52', N'promo53')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod52', N'promo54')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod52', N'promo55')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod52', N'promo56')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod53', N'promo51')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod53', N'promo52')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod54', N'promo51')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod54', N'promo52')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod55', N'promo53')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod55', N'promo54')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod55', N'promo55')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod55', N'promo56')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod56', N'promo53')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod56', N'promo54')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod56', N'promo55')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod56', N'promo56')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod57', N'promo50')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod57', N'promo51')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod57', N'promo52')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod58', N'promo53')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod58', N'promo54')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod58', N'promo55')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod58', N'promo56')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod59', N'promo50')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod59', N'promo51')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod59', N'promo52')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod60', N'promo50')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod60', N'promo51')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod60', N'promo52')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod61', N'promo51')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod61', N'promo52')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod62', N'promo53')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod62', N'promo54')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod62', N'promo55')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod62', N'promo56')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod63', N'promo53')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod63', N'promo54')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod63', N'promo55')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod63', N'promo56')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod64', N'promo53')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod64', N'promo54')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod64', N'promo55')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod64', N'promo56')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod65', N'promo53')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod65', N'promo54')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod65', N'promo55')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod65', N'promo56')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod66', N'promo50')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod66', N'promo51')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod66', N'promo52')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod67', N'promo50')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod67', N'promo51')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod67', N'promo52')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod68', N'promo50')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod68', N'promo51')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod68', N'promo52')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod69', N'promo53')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod69', N'promo54')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod69', N'promo55')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod69', N'promo56')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod70', N'promo53')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod70', N'promo54')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod70', N'promo55')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod70', N'promo56')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod71', N'promo53')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod71', N'promo54')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod71', N'promo55')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod71', N'promo56')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod72', N'promo51')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod72', N'promo52')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod73', N'promo50')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod73', N'promo51')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod73', N'promo52')
GO

INSERT [dbo].[Advertisement] ([Id], [Image]) VALUES (N'adv50', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Default%2FBanner_2%23%23FZd6h?alt=media&token=f311f8a8-4055-469c-a982-06ef65f2acda')
GO
INSERT [dbo].[Advertisement] ([Id], [Image]) VALUES (N'adv51', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Default%2FBanner_8%23%23FZd6w?alt=media&token=8ccd3202-c625-4b0d-a688-94b4cdedb9ff')
GO
INSERT [dbo].[Advertisement] ([Id], [Image]) VALUES (N'adv52', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Default%2FBanner_2%23%23HZd6h?alt=media&token=55b39012-fb7a-4a9f-aa54-a72418972210')
GO
INSERT [dbo].[AdInUse] ([Id], [Position]) VALUES (N'adv51', 1)
GO
INSERT [dbo].[AdInUse] ([Id], [Position]) VALUES (N'adv52', 3)
GO
INSERT [dbo].[AdInUse] ([Id], [Position]) VALUES (N'adv50', 2)
GO
INSERT [dbo].[UserLogin] VALUES (N'shop01', '2023-1-17', N'HFOfer7qw1fOcX/Mekcrs3ImzXyO6+MEDrZnfchBsP0=', N'trin41996@gmail.com', N'HFOfer7qw1fOcX/Mekcrs3ImzXyO6+MEDrZnfchBsP0=', 0)
GO
INSERT [dbo].[UserLogin] VALUES (N'shop02', '2023-1-17', N'uMS3oY91AlY830gSI2xQ/vQ6s8B966Pc77xg7ax1w+w=', N'trin2187@gmail.com', N'lvxt4ggH/USzQIgaN+6Yg+8CnaFiT9sM/NEyd5EEHW4=', 0)
GO

INSERT [dbo].[MUser] ([Id], [Role], [Name], [PhoneNumber], [Email], [Gender], [DOB], [Description], [StatusUser], [StatusShop], [DefaultAddress]) VALUES (N'shop01', N'Shop', N'Karin Shop', N'0123456789', N'trin41996@gmail.com', 0, NULL, N'- Uy tín tạo nên thuong hiệu shop. 
- Giao hàng và hỗ trợ đổi trả trong tuần đầu tiên.
- Có bảo hành và hóa đơn đầy đủ.
- Cam kết chất lượng.', N'NotBanned', N'NotBanned', N'20230115234749')
GO
INSERT [dbo].[MUser] ([Id], [Role], [Name], [PhoneNumber], [Email], [Gender], [DOB], [Description], [StatusUser], [StatusShop], [DefaultAddress]) VALUES (N'shop02', N'Shop', N'Hanabi Shop', N'0123456789', N'trin2187@gmail.com', 0, NULL, N'- Chuyên các mặt hàng thời trang hiện đại, uy tín, chất lượng. 
- Giao hàng và hỗ trợ đổi trả trong tuần đầu tiên.
- Có bảo hành và hóa đơn đầy đủ.
- Cam kết chất lượng.', N'NotBanned', N'NotBanned', N'20230115235733')
GO


INSERT [dbo].[Address] ([Id], [IdUser], [Name], [PhoneNumber], [Address], [Status]) VALUES (N'20230117191829', N'shop01', N'Karin', N'0123456789', N'023/PT, Phước Ngãi, Ba Tri, Bến Tre', 1)
GO
INSERT [dbo].[Address] ([Id], [IdUser], [Name], [PhoneNumber], [Address], [Status]) VALUES (N'20230117192108', N'shop02', N'Hanabi', N'0123456789', N'Linh Trung, Thủ Đức', 1)
GO

INSERT [dbo].[Category] ([Id], [Name], [Status]) VALUES (N'cat01', N'Jeans', N'NotBanned')
GO
INSERT [dbo].[Category] ([Id], [Name], [Status]) VALUES (N'cat02', N'Polo', N'NotBanned')
GO
INSERT [dbo].[Category] ([Id], [Name], [Status]) VALUES (N'cat03', N'Hoodie', N'NotBanned')
GO
INSERT [dbo].[Brand] ([Id], [Name], [Status]) VALUES (N'brand01', N'Clownz', N'NotBanned')
GO
INSERT [dbo].[Brand] ([Id], [Name], [Status]) VALUES (N'brand02', N'Teelab', N'NotBanned')
GO
INSERT [dbo].[Brand] ([Id], [Name], [Status]) VALUES (N'brand03', N'LADOS', N'NotBanned')
GO

INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod01', N'Polo Teelab Special', N'cat02', N'brand02', N'shop01', 90, 0, 30, 0, 0, 0, 0, 1, 1, 1, N'Night Black', N'- Chất liệu: TC cá sấu

- Form: Cơ bản

- Thiết kế: Hình Typhography In cán lụa cao cấp
', CAST(N'2023-01-17T13:21:00' AS SmallDateTime), 0)
GO

INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod02', N'Polo Unisex Teelab Essentials Line', N'cat02', N'brand02', N'shop01', 90, 0, 30, 0, 0, 0, 0, 1, 1, 1, N'White', N'- Chất liệu: TC cá sấu

- Form: Cơ bản

- Thiết kế: Hình Typhography In cán lụa cao cấp
', CAST(N'2023-01-17T13:21:00' AS SmallDateTime), 0)
GO

INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod03', N'Polo Unisex Teelab Vintage Logo AP019', N'cat02', N'brand02', N'shop01', 110, 0, 30, 0, 0, 0, 0, 1, 1, 1, N'Green', N'- Chất liệu: TC cá sấu

- Form: Cơ bản

- Thiết kế: Hình Typhography In cán lụa cao cấp
', CAST(N'2023-01-17T13:21:00' AS SmallDateTime), 0)
GO

INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod04', N'Polo Unisex Teelab Kitten Embroidered AP022', N'cat02', N'brand02', N'shop01', 110, 0, 30, 0, 0, 0, 0, 1, 1, 1, N'Dark, Brown', N'- Chất liệu: TC cá sấu

- Form: Cơ bản

- Thiết kế: Hình Typhography In cán lụa cao cấp
', CAST(N'2023-01-17T13:21:00' AS SmallDateTime), 0)
GO

INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod05', N'Polo Teelab Twin Hearts AP020', N'cat02', N'brand02', N'shop01', 110, 0, 30, 0, 0, 0, 0, 1, 1, 1, N'Brown', N'- Chất liệu: TC cá sấu

- Form: Cơ bản

- Thiết kế: Hình Typhography In cán lụa cao cấp
', CAST(N'2023-01-17T13:21:00' AS SmallDateTime), 0)
GO

INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod06', N'Polo LADOS-9089', N'cat02', N'brand03', N'shop01', 90, 0, 30, 0, 0, 0, 1, 1, 1, 1, N'Black, White', N'- Chất liệu: TC cá sấu

- Form: Cơ bản

- Thiết kế: Hình Typhography In cán lụa cao cấp
', CAST(N'2023-01-17T13:21:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod07', N'Polo Sport LADOS-9090', N'cat02', N'brand03', N'shop01', 99, 0, 30, 0, 0, 0, 1, 1, 1, 1, N'Black, White, Grey, Blue, Green', N'- Chất liệu: TC cá sấu

- Form: Cơ bản

- Thiết kế: Hình Typhography In cán lụa cao cấp
', CAST(N'2023-01-17T13:21:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod08', N'Polo Sport LADOS-9008', N'cat02', N'brand03', N'shop01', 99, 0, 30, 0, 0, 0, 1, 1, 1, 1, N'White, Blue', N'- Chất liệu: TC cá sấu

- Form: Cơ bản

- Thiết kế: Hình Typhography In cán lụa cao cấp
', CAST(N'2023-01-17T13:21:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod09', N'Hoodie LADOS-9888', N'cat03', N'brand03', N'shop01', 99, 0, 30, 0, 1, 0, 0, 0, 0, 0, N'Black, Grey', N'- Chất liệu: Vải

- Form: Cơ bản

- Thiết kế: Dầy, ấm
', CAST(N'2023-01-17T13:21:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod10', N'Hoodie Teelab TGOD', N'cat03', N'brand02', N'shop01', 99, 0, 30, 0, 1, 0, 0, 0, 0, 0, N'Green, Beige', N'- Chất liệu: Vải

- Form: Cơ bản

- Thiết kế: Dầy, ấm
', CAST(N'2023-01-17T13:21:00' AS SmallDateTime), 0)
GO

INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod11', N'Jean LADOS-4073', N'cat01', N'brand03', N'shop01', 99, 0, 30, 0, 1, 0, 0, 0, 0, 0, N'Blue, Dark Blue', N'- Chất liệu: Jean

- Form: Cơ bản

- Thiết kế: Mát, Dễ chịu, Hơi bó
', CAST(N'2023-01-17T13:21:00' AS SmallDateTime), 0)
GO


INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod12', N'Jean LADOS-4068', N'cat01', N'brand03', N'shop01', 179, 0, 30, 0, 1, 0, 0, 0, 0, 0, N'Blue, Dark Blue', N'- Chất liệu: Jean

- Form: Cơ bản

- Thiết kế: Mát, Dễ chịu, Hơi bó
', CAST(N'2023-01-17T13:21:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod13', N'Hoodie Clownz Melting Face', N'cat03', N'brand01', N'shop02', 199, 0, 30, 0, 1, 0, 0, 0, 0, 0, N'Grey, Beige', N'- Chất liệu: Vải

- Form: Cơ bản

- Thiết kế: Dầy, ấm
', CAST(N'2023-01-17T13:21:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod14', N'Hoodie Clownz Smiley Face', N'cat03', N'brand01', N'shop02', 199, 0, 30, 0, 1, 0, 0, 0, 0, 0, N'Grey, Green, Black', N'- Chất liệu: Vải

- Form: Cơ bản

- Thiết kế: Dầy, ấm
', CAST(N'2023-01-17T13:21:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod15', N'Hoodie Clownz Big Logo V2', N'cat03', N'brand01', N'shop02', 219, 0, 30, 0, 1, 0, 0, 0, 0, 0, N'Grey, Beige', N'- Chất liệu: Vải

- Form: Cơ bản

- Thiết kế: Dầy, ấm
', CAST(N'2023-01-17T13:21:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod16', N'Hoodie Clownz Basic V3', N'cat03', N'brand01', N'shop02', 199, 0, 30, 0, 1, 0, 0, 0, 0, 0, N'Black, Beige', N'- Chất liệu: Vải

- Form: Cơ bản

- Thiết kế: Dầy, ấm
', CAST(N'2023-01-17T13:21:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod17', N'Hoodie Clownz Homemade Rapper', N'cat03', N'brand01', N'shop02', 219, 0, 30, 0, 1, 0, 0, 0, 0, 0, N'Black, White', N'- Chất liệu: Vải

- Form: Cơ bản

- Thiết kế: Dầy, ấm
', CAST(N'2023-01-17T13:21:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod18', N'Hoodie Clownz Zipped Basic', N'cat03', N'brand01', N'shop02', 199, 0, 30, 0, 1, 0, 0, 0, 0, 0, N'Black, Green', N'- Chất liệu: Vải

- Form: Cơ bản

- Thiết kế: Dầy, ấm
', CAST(N'2023-01-17T13:21:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod19', N'Polo CLownz Color Blocked', N'cat02', N'brand01', N'shop02', 149, 0, 30, 0, 0, 0, 0, 1, 1, 1, N'Yellow, Brown', N'- Chất liệu: Vải

- Form: Rộng

- Thiết kế: Tay lở, rộng rãi, thoải mái
', CAST(N'2023-01-17T13:21:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod20', N'Polo CLownz Classic', N'cat02', N'brand01', N'shop02', 149, 0, 30, 0, 0, 0, 0, 1, 1, 1, N'Black, White', N'- Chất liệu: Vải

- Form: Rộng

- Thiết kế: Tay lở, rộng rãi, thoải mái
', CAST(N'2023-01-17T13:21:00' AS SmallDateTime), 0)
GO

INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod21', N'Polo CLownz Diagonal', N'cat02', N'brand01', N'shop02', 179, 0, 30, 0, 0, 0, 0, 1, 1, 1, N'Green, Purple', N'- Chất liệu: Vải

- Form: Rộng

- Thiết kế: Tay lở, rộng rãi, thoải mái
', CAST(N'2023-01-17T13:21:00' AS SmallDateTime), 0)
GO

INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod22', N'Polo CLownz Graffity Tag', N'cat02', N'brand01', N'shop02', 149, 0, 30, 0, 0, 0, 0, 1, 1, 1, N'Black, White', N'- Chất liệu: Vải

- Form: Rộng

- Thiết kế: Tay lở, rộng rãi, thoải mái
', CAST(N'2023-01-17T13:21:00' AS SmallDateTime), 0)
GO

INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod23', N'Jean Clownz Straight', N'cat01', N'brand01', N'shop02', 179, 0, 30, 0, 1, 0, 0, 0, 0, 0, N'Blue', N'- Chất liệu: Jean

- Form: Cơ bản

- Thiết kế: Mát, Dễ chịu
', CAST(N'2023-01-17T13:21:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod24', N'Jean Clownz Racing Flame', N'cat01', N'brand01', N'shop02', 179, 0, 30, 0, 1, 0, 0, 0, 0, 0, N'Blue', N'- Chất liệu: Jean

- Form: Cơ bản

- Thiết kế: Mát, Dễ chịu
', CAST(N'2023-01-17T13:21:00' AS SmallDateTime), 0)
GO

INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod01', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod01%2FImage?alt=media&token=3e5ee2e9-73a9-43ae-a2df-db398b219ef8')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod01', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod01%2FImage_100?alt=media&token=e8f65d8c-0f44-45da-b973-914e0008595a')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod01', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod01%2FImage_172?alt=media&token=f02467a6-5213-4241-8cfb-2c3f9000658c')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod02', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod02%2FImage?alt=media&token=8a50868d-058d-4459-96da-89c7166012af')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod03', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod03%2FImage?alt=media&token=c2678dbb-cf41-4348-a39d-07257101cb76')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod04', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod04%2FImage?alt=media&token=f647215e-a6aa-4bab-b55d-268749bd1f4e')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod04', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod04%2FImage_180?alt=media&token=899a40c6-128d-44ce-a362-8748cbaaf97e')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod04', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod04%2FImage_28?alt=media&token=6d9bcffa-576f-4c57-b173-90219fd53ca3')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod05', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod05%2FImage?alt=media&token=87d7eb9d-e199-4f51-8df0-03f3c3aabd24')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod05', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod05%2FImage_26?alt=media&token=d239dc77-fd62-4215-ab65-1dbbeed4b06d')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod05', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod05%2FImage_46?alt=media&token=00f57c3a-50fe-4808-881e-b983b8fd8bca')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod06', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod06%2FImage?alt=media&token=7ca450a7-f6bd-4028-a5ad-dc42986b3bfa')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod06', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod06%2FImage_68?alt=media&token=9ceee3be-0b71-4add-a6e8-90f11e50e0f1')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod06', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod06%2FImage_80?alt=media&token=1fd8c9b6-a679-4dc7-ba0a-0bf8017041a3')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod07', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod07%2FImage?alt=media&token=75163e79-0f66-4042-b74f-64fb05be9c3b')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod07', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod07%2FImage_114?alt=media&token=5e4c018d-ff3c-483f-9424-6c5b0d556b55')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod07', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod07%2FImage_134?alt=media&token=771d6dbb-fb99-4fbf-8eba-329723c3e959')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod07', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod07%2FImage_144?alt=media&token=749e4e54-58c9-4480-87c1-8ad91cfe62aa')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod07', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod07%2FImage_194?alt=media&token=2f055f06-dbec-4dd5-b7f8-202b4d666f56')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod07', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod07%2FImage_6?alt=media&token=f76d2d11-2d77-4b96-a25c-27c2a8fcfa7c')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod08', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod08%2FImage?alt=media&token=9f2b1465-c554-472b-961a-91e3ee872721')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod08', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod08%2FImage_44?alt=media&token=72db1380-bd0c-46cc-bd16-8e77a38dc3e2')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod08', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod08%2FImage_46?alt=media&token=1d6c9d42-61af-49ce-ac3b-56be8a489721')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod09', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod09%2FImage?alt=media&token=05728796-7993-4e4d-b9bb-537babc28ff2')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod09', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod09%2FImage_170?alt=media&token=ed23d65e-4cc4-4ade-bda8-16a4e4eb2a2f')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod09', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod09%2FImage_72?alt=media&token=9c2b32cc-5210-4ff1-be16-ec11287a83ad')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod10', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod10%2FImage?alt=media&token=4d2caace-9214-4be8-93f4-e5c37ee1cba9')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod10', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod10%2FImage_48?alt=media&token=330396eb-1e25-4651-8d59-46b9ef821a8d')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod10', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod10%2FImage_60?alt=media&token=abbe720f-7327-4fbd-9684-1124833ac5e7')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod11', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod11%2FImage?alt=media&token=a7e44c1f-e5b5-4d50-965c-9312e9d44415')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod11', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod11%2FImage_182?alt=media&token=59c4b8c9-286a-4bb9-94d7-ca0bdb04bf2b')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod11', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod11%2FImage_40?alt=media&token=f7aa125b-23b7-4e88-8da7-de1e6d8a4f4b')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod12', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod12%2FImage?alt=media&token=c7739599-0f79-40c7-af5e-2967ea993c76')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod12', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod12%2FImage_184?alt=media&token=740725e4-924f-4fd9-8429-78101d994315')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod12', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod12%2FImage_36?alt=media&token=f330f896-ea64-43ad-bdbf-04bbfc6f0ee2')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod13', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod13%2FImage?alt=media&token=ff0929e1-142c-4500-b0ba-1370ecd75ffe')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod13', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod13%2FImage_42?alt=media&token=a4931696-7cc1-4ce7-9553-7efb95c19d46')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod14', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod14%2FImage?alt=media&token=1700be0c-d1a4-4c55-8448-c004affd3cdf')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod14', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod14%2FImage_62?alt=media&token=0d17ba76-8e78-4285-a96e-80e03cc33763')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod14', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod14%2FImage_72?alt=media&token=9f341d1c-54fe-4347-9233-184eff9a4598')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod15', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod15%2FImage?alt=media&token=2121a36e-297b-4a28-a6e4-154554c9b110')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod15', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod15%2FImage_66?alt=media&token=f7561736-d66d-47d6-9339-2ca32402d9b6')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod16', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod16%2FImage?alt=media&token=d4f50d98-f376-4ae2-ad7b-73099bb99a09')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod16', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod16%2FImage_98?alt=media&token=addd29fd-69e6-46fe-9419-275bae7fde78')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod17', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod17%2FImage?alt=media&token=dc4f404c-67f9-41a3-bb4e-450fea2073c8')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod17', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod17%2FImage_120?alt=media&token=463c389d-e5b3-49f0-89ad-e5e2b4b0e4b0')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod18', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod18%2FImage?alt=media&token=330f3a2b-10ca-4969-b039-f107d2c30c7d')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod18', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod18%2FImage_108?alt=media&token=e16e711f-bc43-46f0-9596-f4d371d0cd0c')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod19', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod19%2FImage?alt=media&token=02a3ff51-1c4a-4365-ba13-9cd9727d51e4')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod19', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod19%2FImage_66?alt=media&token=03cdebff-8452-447d-9d88-32c3a3016b51')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod20', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod20%2FImage?alt=media&token=1d07d6bd-e009-4f41-a0d0-46e02fec288c')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod20', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod20%2FImage_60?alt=media&token=42a46ba0-0a84-4c49-aca4-45104bf65674')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod21', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod21%2FImage?alt=media&token=ee7ecff4-d2d3-43d4-ac1e-a49bde3a1abc')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod21', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod21%2FImage_186?alt=media&token=86327783-51fe-4f5f-897a-c51a6efb8e0e')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod22', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod22%2FImage?alt=media&token=641addc5-6431-4867-80e8-f6cc28085107')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod22', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod22%2FImage_0?alt=media&token=644f8845-e639-4e40-810c-ebafea283f84')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod23', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod23%2FImage?alt=media&token=331ce4ea-0957-4249-9bcb-bb5cb5984bc5')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod23', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod23%2FImage_176?alt=media&token=5f36b857-590e-4696-9a56-45dcb484cdd8')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod24', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod24%2FImage?alt=media&token=5cdbe798-b486-419b-9774-54cc6193235c')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod24', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2Fprod24%2FImage_48?alt=media&token=ace53bae-3437-4735-8655-6824ca55c3e7')
GO

INSERT [dbo].[Brand] ([Id], [Name], [Status]) VALUES (N'br025', N'Sinsay', N'NotBanned')
GO
INSERT [dbo].[Brand] ([Id], [Name], [Status]) VALUES (N'br026', N'The North Face', N'NotBanned')
GO
INSERT [dbo].[Brand] ([Id], [Name], [Status]) VALUES (N'br027', N'Reebok', N'NotBanned')
GO

INSERT [dbo].[Category] ([Id], [Name], [Status]) VALUES (N'cate25', N'Sweatshirts', N'NotBanned')
GO
INSERT [dbo].[Category] ([Id], [Name], [Status]) VALUES (N'cate26', N'Jackets', N'NotBanned')
GO
INSERT [dbo].[Category] ([Id], [Name], [Status]) VALUES (N'cate27', N'Shoes', N'NotBanned')
GO

INSERT [dbo].[UserLogin] ([IdUser], [CreatedDate], [Password], [Username], [Salt], [Provider]) VALUES (N'usr025', CAST(N'2023-01-16T00:00:00.000' AS DateTime), N'97Cm+9mxidJNqRRQYTBX91dTboeHH5I1Wxb/qEouGx8=', N'21521495@gm.uit.edu.vn', N'Pm1hGhmUURIuUZhL47Za2IyHWuOWkMIb6uoAcJQkGNs=', 0)
GO
INSERT [dbo].[UserLogin] ([IdUser], [CreatedDate], [Password], [Username], [Salt], [Provider]) VALUES (N'usr026', CAST(N'2023-01-16T00:00:00.000' AS DateTime), N'laqw1vR1ZJtg/f0p8oOUtF8RjdcfmH3LRFmQAmaTI/w=', N'isitaniceday@gmail.com', N'aMuYjAB+AZqZV+7k7964jnF5RWK3bFViDValmS1GuRw=', 0)
GO

INSERT [dbo].[MUser] ([Id], [Role], [Name], [PhoneNumber], [Email], [Gender], [DOB], [Description], [StatusUser], [StatusShop], [SourceImageAva], [SourceImageBackground], [DefaultAddress]) VALUES (N'usr025', N'Shop', N'Winter Clothing', N'0135798642', N'21521495@gm.uit.edu.vn', 1, NULL, N'We are committed to making the best products on earth—and keeping Mother Nature, our communities and the future in focus.', N'NotBanned', N'NotBanned', NULL, NULL, 'add26')
GO
INSERT [dbo].[MUser] ([Id], [Role], [Name], [PhoneNumber], [Email], [Gender], [DOB], [Description], [StatusUser], [StatusShop], [SourceImageAva], [SourceImageBackground], [DefaultAddress]) VALUES (N'usr026', N'Shop', N'Casual', N'0796789123', N'isitaniceday@gmail.com', 1, NULL, N'MORE SUSTAINABLE PRODUCTION', N'NotBanned', N'NotBanned', NULL, NULL, 'add25')
GO

INSERT [dbo].[Address] ([Id], [IdUser], [Name], [PhoneNumber], [Address], [Status]) VALUES (N'add25', N'usr026', N'Formal', N'024683579', N' 38 Dang Van Ngu St., Lane 4, Dong Da Dist., Hanoi', 1)
GO
INSERT [dbo].[Address] ([Id], [IdUser], [Name], [PhoneNumber], [Address], [Status]) VALUES (N'add26', N'usr025', N'Summer', N'0987654321', N'13402 Behrens Ave, Norwalk, California', 1)
GO

INSERT [dbo].[CategoryRequest] ([Id], [IdShop], [Name], [Reason]) VALUES (N'carq26', N'usr025', N'Boots', N'To specify the boots and category shoes')
GO

INSERT [dbo].[BrandRequest] ([Id], [IdShop], [Name], [Reason]) VALUES (N'brq25', N'usr025', N'Uniqlo', N'Our shop may sale the products of that brand')
GO

INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod48', N'Classics Vector Track Jacket', N'cate26', N'br027', N'usr026', 90, 0, 30, 0, 0, 0, 0, 1, 1, 1, N'Night Black, Forest Green, Vector Navy', N'Minimalist style, maximum versatility. This Reebok track jacket pays homage to the classic Vector logo for a touch of old-school cool. Lightweight, crinkled material brings unexpected texture to your look for effortless comfort and style.', CAST(N'2023-01-17T13:21:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod47', N'Question Mid Basketball Shoes - Preschool', N'cate27', N'br027', N'usr026', 75, 0, 50, 0, 0, 1, 1, 1, 1, 1, N'Fluid Blue - Toxic Yellow', N'Allen Iverson''s departure from Philly posed one big question: Can he make it work with his new team? The classic style on these boys'' Reebok basketball-inspired shoes answers that loud and clear. Vibrant, eye-catching contrast-color accents reflect his new destination.', CAST(N'2023-01-17T13:09:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod46', N'Sweatshirt with print', N'cate25', N'br025', N'usr026', 20, 5, 20, 0, 0, 1, 1, 1, 1, 1, N'Navy, Hot Pink', N'- relaxed design
- round neck
- long sleeves
- soft and fluffy finish to the inside', CAST(N'2023-01-17T15:27:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod45', N'Basic sweatshirt', N'cate25', N'br025', N'usr026', 23, 0, 100, 0, 0, 1, 1, 1, 1, 0, N'Mid Grey, Black, Blue', N'- round neck
- long sleeves with dropped shoulders
- soft and fluffy finish to the inside
- elasticated trim', CAST(N'2023-01-17T13:02:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod44', N'Women’s VECTIV™ Escape I FUTURELIGHT™ Shoes', N'cate27', N'br026', N'usr026', 151, 0, 10, 0, 0, 1, 1, 1, 1, 1, N'Gardenia White', N'The Women’s VECTIV™ Escape I FUTURELIGHT™ Shoes leverage the forward propulsion of VECTIV™ and breathable-waterproof FUTURELIGHT™ for light, versatile shoes that''ll take your exploration to new heights.', CAST(N'2023-01-17T15:54:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod43', N'Women’s Chilkat V 400 Waterproof Boots', N'cate27', N'br026', N'usr025', 159, 0, 50, 0, 0, 1, 1, 1, 1, 1, N'Black - Vanadis Grey, Deep Taupe - Black', N'Updated for more underfoot comfort and a lighter weight, the Women’s Chilkat V 400 Waterproof Boots still boast our warmest package of toasty Heatseeker™ Eco insulation for comfort on cold winter adventures.', CAST(N'2023-01-17T12:05:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod42', N'Graphic Injection Crew', N'cate25', N'br026', N'usr026', 55, 10, 20, 0, 0, 1, 1, 1, 1, 1, N'Gardenia White - Wild Ginger', N'With an array of front, back, and sleeve graphics, the Women’s Graphic injection Crew is a cool weather sweater that you can match to your style.', CAST(N'2023-01-17T15:22:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod41', N'Tanager Jacket', N'cate26', N'br026', N'usr026', 179, 0, 20, 0, 0, 1, 1, 1, 1, 1, N'Utility Brown Paintbrush Print, Black, Patina Green', N'Whether you’re taking park laps or hiking in search of an untracked line, the Tanager Jacket has the style and waterproof, breathable performance you need.', CAST(N'2023-01-17T15:48:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod40', N'Kids’ Printed 1996 Retro Nuptse Jacket', N'cate26', N'br026', N'usr025', 230, 0, 50, 0, 0, 1, 1, 1, 1, 1, N'Lapis Blue, Cameo Pink', N'The moment you see the oversized baffles, you know you’re looking at our iconic Nuptse. This warm, durable jacket has lofty 700-fill down, a water-repellent finish, and is now available in smaller sizes. It also features 100% recycled fabrics and a design inspired by the classic 1996 version. We’ve updated our youth sizing. Please use our sizing guide to find your best fit.', CAST(N'2023-01-17T12:09:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod39', N'Kids’ Denali Jacket', N'cate26', N'br026', N'usr025', 109, 0, 50, 0, 0, 1, 1, 1, 1, 0, N'Ponderosa Green, Lavender Fog, Cameo Pink', N'Even the tiniest explorers should have iconic style. The durable, water-repellent Kids’ Denali Jacket has all the features needed for a cold-weather outing, whether it’s a trip to the store or chilly night at camp.', CAST(N'2023-01-17T12:25:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod38', N'STAY WARM sherpa sweatshirt', N'cate25', N'br025', N'usr026', 120, 10, 10, 0, 0, 0, 1, 1, 0, 0, N'Grey BLue', N'- stand up collar
- zip fastening
- long sleeves
- kangaroo pocket
- elasticated trim', CAST(N'2023-01-17T13:28:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod37', N'Vanadis Grey - Brilliant Coral', N'cate27', N'br026', N'usr026', 145, 0, 10, 0, 0, 1, 1, 1, 1, 1, N'Vanadis Grey - Brilliant Coral', N'The lightweight Men’s Wayroute Mid FUTURELIGHT™ Boots are ankle-high, trail-ready, and ideal for shorter, active hikes. With progressive styling and sneaker-like comfort, these breathable-waterproof boots will take you on bold new adventures.', CAST(N'2023-01-17T13:51:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod36', N'Baseball sweatshirt', N'cate25', N'br025', N'usr026', 48, 0, 20, 0, 0, 1, 1, 1, 1, 0, N'Dark Brown, Navy', N'- snap fastening
- 2 pockets
- decorative embroidery
- soft and fluffy finish to the inside
- elasticated trim', CAST(N'2023-01-17T15:30:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod35', N'Women’s ThermoBall™ Lace Up Waterproof Boots', N'cate27', N'br026', N'usr026', 139, 0, 20, 0, 0, 1, 1, 1, 1, 1, N'Wasabi - Vanadis Grey, Gardenia White - Silver Grey, Black - Gardenia White, Ponderosa Green', N'The lightweight, waterproof Women’s ThermoBall™ Lace Up Waterproof are secure-fitting winter boots with durable recycled ripstop uppers and toasty ThermoBall™ Eco insulation. Due to the nature of the FTW material, the color execution might not exactly match the color execution in apparel, equipment and accessories.', CAST(N'2023-01-17T16:08:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod34', N'Classic Leather V2 Shoes - Grade School', N'cate27', N'br027', N'usr026', 71, 5, 20, 0, 0, 1, 1, 1, 1, 1, N'Whisper Blue, Pink Fushion', N'Pushing proportions and design, these junior girls'' Classic Leather V2 shoes incorporate iconic DNA with modernity, boldness and flare. The molded Vector and oversized outsole lugs pay homage to Reebok.', CAST(N'2023-01-17T13:15:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod33', N'Toddler Alpenglow II Boots', N'cate27', N'br026', N'usr025', 55, 0, 100, 0, 0, 1, 1, 1, 1, 1, N'Fuschia Pink - Coral Sunrise, Wasabi - Patina Green, Acoustic Blue - Shady Blue, Black - Zinc Grey', N'The Toddler Alpenglow II Boots provide versatile winter protection and warmth with Heatseeker™ Eco insulation and flexible, toddler-friendly construction.', CAST(N'2023-01-17T12:33:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod25', N'Women’s New Outerboroughs Parka', N'cate26', N'br026', N'usr025', 500, 0, 50, 0, 0, 1, 1, 1, 1, 1, N'Ponderosa Green, Cordovan', N'The Women’s New Outerboroughs Parka features waterproof, breathable DryVent™ 2L, down insulation and a windproof fabric. This relaxed-fit jacket delivers toasty-warmth in icy conditions', CAST(N'2023-01-16T23:58:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod26', N'Women’s Alpine Polartec® 200 Full-Zip Jacket', N'cate26', N'br026', N'usr025', 119, 0, 50, 0, 0, 1, 1, 1, 1, 1, N'Black, Wasabi', N'Cold-weather days call for reliable protection. The Women’s Alpine Polartec® 200 Full-Zip Jacket has four-way stretch, hem adjustability, and is made from 100% recycled fabrics that are designed to be reused and repurposed at the end of their lifecycle.', CAST(N'2023-01-16T23:58:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod27', N'Women’s ThermoBall™ Eco Triclimate® Parka', N'cate26', N'br026', N'usr025', 500, 0, 50, 0, 0, 1, 1, 1, 1, 1, N'Lavender Fog, Wild Ginger', N'Choose your own adventure in the versatile Women’s ThermoBall™ Eco Triclimate® Parka. The outer jacket is made with waterproof-breathable DryVent™, while a unique quilting pattern on the removable liner jacket means increased warmth and style, wherever your exploration leads.', CAST(N'2023-01-16T23:58:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod28', N'Women’s Belleview Stretch Down Jacket', N'cate26', N'br026', N'usr025', 269, 0, 50, 0, 0, 1, 1, 1, 1, 1, N'Shady Blue', N'Slim-fit with two-way stretch for enhanced range of motion, the Women’s Belleview Stretch Down Jacket combines classic aesthetics with technical features. With the ability to pack down into its own pocket, it’s the packable adventure layer you’ve been searching for.', CAST(N'2023-01-16T23:58:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod29', N'Women’s Summit Series Pumori FUTURELIGHT™ Jacket', N'cate26', N'br026', N'usr025', 269, 0, 50, 0, 0, 1, 1, 1, 1, 1, N'Black, Wasabi', N'Cold-weather days call for reliable protection. The Women’s Alpine Polartec® 200 Full-Zip Jacket has four-way stretch, hem adjustability, and is made from 100% recycled fabrics that are designed to be reused and repurposed at the end of their lifecycle.', CAST(N'2023-01-16T23:58:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod30', N'Men’s Chilkat V Zip Waterproof Boots', N'cate27', N'br026', N'usr025', 269, 0, 50, 0, 0, 1, 1, 1, 1, 1, N'Black - Asphalt Grey, Coffee Brown - Black', N'The progressive Men’s Chilkat V Zip Waterproof Boots are the newest addition to our much-loved Chilkat line. These durable winter boots deliver waterproof comfort and warmth and have a zippered closure for easy on/off.', CAST(N'2023-01-16T23:58:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod31', N'Women’s Nuptse Short Jacket', N'cate26', N'br026', N'usr025', 269, 0, 50, 0, 0, 1, 1, 1, 1, 1, N'Lavender Fog, Harbor Blue, Dark Oak', N'Undeniable urban style from the second you see it, the relaxed-fit, water-repellent Women’s Nuptse Short Jacket will open the door to new city adventures.', CAST(N'2023-01-16T23:58:00' AS SmallDateTime), 0)
GO
INSERT [dbo].[Product] ([Id], [Name], [IdCategory], [IdBrand], [IdShop], [Price], [Sale], [InStock], [Sold], [IsOneSize], [IsHadSizeS], [IsHadSizeM], [IsHadSizeL], [IsHadSizeXL], [IsHadSizeXXL], [Color], [Description], [DateOfSale], [BanLevel]) VALUES (N'prod32', N'Women’s Printed 71 Sierra Down Short Jacket', N'cate26', N'br026', N'usr025', 269, 0, 50, 0, 0, 1, 1, 1, 1, 1, N'Ponderosa Green, Black Groovy Print', N'Retro style meets modern upgrades with the Women’s Printed 71 Sierra Down Short Jacket. A fully-recycled, water-repellent body and 600-fill down insulation help keep you dry and toasty warm. A three-piece removable hood provides coverage when you need it and less bulk when you don’t.', CAST(N'2023-01-16T23:58:00' AS SmallDateTime), 0)
GO

INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod48', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F0%23%23cZd6b_10?alt=media&token=843b1007-b9a4-49e8-9de8-5ac846e17138')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod48', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F0%23%23cZd6b_168?alt=media&token=fe602f67-2fcc-488f-bc07-492e7b02a69a')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod48', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F0%23%23cZd6b_18?alt=media&token=9806c988-c2ac-4300-abf0-f7412c37ca78')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod47', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F0%23%23YZd6a?alt=media&token=3a2d3ac4-f1f4-4fe1-8c6c-81c71d96c9b0')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod47', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F0%23%23YZd6a_116?alt=media&token=65ef441e-c2f5-4899-b899-4894b8c955fd')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod47', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F0%23%23YZd6a_94?alt=media&token=f0188b04-fb26-4660-b882-cf57bda831a2')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod46', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F1%23%23iZd6d?alt=media&token=53bc3bf2-ab17-471f-a402-32b7465d2b6a')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod46', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F1%23%23iZd6d_102?alt=media&token=7dd6cc13-4f5c-4f1c-9546-3222eff0fb19')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod45', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F1%23%23XZd6d?alt=media&token=335627b9-9d96-4ee6-8f30-22809af1b551')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod45', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F1%23%23XZd6d_122?alt=media&token=59a587d4-43f2-4b33-8d2d-26d42a99f160')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod45', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F1%23%23XZd6d_170?alt=media&token=ff1bdf3f-f4e6-4d94-95be-853b16f2cb3d')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod44', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F2%23%23nZd6f?alt=media&token=82cb9e92-a83d-47de-801d-22590970d5de')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod44', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F2%23%23nZd6f_194?alt=media&token=462423e9-64b8-4923-a950-fcc9eebbf569')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod43', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F2%23%23QZd6g?alt=media&token=287c1e37-6124-4be2-abf9-c16745a566ea')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod43', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F2%23%23QZd6g_148?alt=media&token=d02abf37-b3c4-489f-a5ed-dcee39ed16ba')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod42', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F3%23%23hZd6j?alt=media&token=bb01c9fd-1013-4a52-a9e1-8dc64f099194')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod42', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F3%23%23hZd6j_24?alt=media&token=f410dcf6-4c49-4a96-83bc-c9faf4916aa9')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod41', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F3%23%23lZd6i?alt=media&token=c780d18a-f3ba-4b9e-af29-60c8145e9f12')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod41', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F3%23%23lZd6i_12?alt=media&token=1ba70810-14b7-4838-8608-acd8e43d6ce0')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod41', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F3%23%23lZd6i_128?alt=media&token=a9fb41f3-72c8-4773-bd30-d8bd485286e1')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod40', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F4%23%23SZd6k?alt=media&token=97c3908f-f6e0-4314-a915-6c00c2d55913')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod40', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F4%23%23SZd6k_152?alt=media&token=95c8c629-0103-42d8-9aab-71e74f8d78bc')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod40', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F4%23%23SZd6k_94?alt=media&token=5d1ea67e-73f7-426b-9d52-25aee3e9df3b')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod39', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F5%23%23UZd6o?alt=media&token=c33cc093-128e-4d80-99da-55620cf09a1f')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod39', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F5%23%23UZd6o_136?alt=media&token=d01b76cd-a2af-4f0b-b97a-b1ab3e9b7c46')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod39', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F5%23%23UZd6o_88?alt=media&token=3bddf6c7-6c75-45f3-acd1-0077aa28f391')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod38', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F6%23%23dZd6q_408?alt=media&token=b3f326c2-59d4-41cb-a08e-590501876a7f')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod38', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F6%23%23dZd6q_88?alt=media&token=44dc093f-3ae4-4f21-ab0b-18dd453e1ccc')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod37', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F8%23%23fZd6x?alt=media&token=d4cff5c1-dce6-4aeb-9167-c1e1e88dcb36')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod37', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F8%23%23fZd6x_112?alt=media&token=fbed8349-605d-4f61-8f10-340308c15a29')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod37', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F8%23%23fZd6x_178?alt=media&token=73e1870f-d343-486c-b55a-df73c442b656')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod37', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F8%23%23fZd6x_60?alt=media&token=a86a5bcf-1e45-4026-a209-f6bcb7170125')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod36', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F8%23%23kZd6x?alt=media&token=4a7b2191-5a59-4c4b-b95b-cb24d054c8d5')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod36', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F8%23%23kZd6x_64?alt=media&token=2e92da40-931d-49fa-a401-04dec3e5b683')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod35', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F8%23%23pZd6v?alt=media&token=ab19f8f2-f8ba-4b56-884a-73dcb6d2bd96')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod35', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F8%23%23pZd6v_106?alt=media&token=a95951fe-06f1-48cf-897c-850961b97794')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod35', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F8%23%23pZd6v_160?alt=media&token=9fdcf49c-6474-4544-8163-7f9bc6451ce5')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod35', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F8%23%23pZd6v_80?alt=media&token=57874f77-eb74-4f7f-beb6-38befcc28dc6')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod34', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F9%23%23aZd6y?alt=media&token=48a93448-ed87-475c-a6e7-8b8526710ee9')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod34', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F9%23%23aZd6y_76?alt=media&token=35e082e7-2794-46d2-a2da-caeaabd01398')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod33', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F9%23%23VZd6y?alt=media&token=e6b3cfce-defb-4679-8644-ee67ef44953e')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod33', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F9%23%23VZd6y_10?alt=media&token=6bdfdc16-96cf-4d0f-a68a-97828f1aa487')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod33', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F9%23%23VZd6y_116?alt=media&token=3d02620e-3d6a-4826-a564-1bb2c9dcaad0')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod33', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F9%23%23VZd6y_120?alt=media&token=470f5ce8-c422-40be-9df6-8bc0d0fe423d')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod25', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F6%23%23iZd6r?alt=media&token=1621faf3-d5c2-435a-a5b9-5b0c4d433d10')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod25', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F6%23%23iZd6r_46?alt=media&token=5157fdbd-7b18-42c4-9458-364761c20d10')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod25', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F6%23%23iZd6r_60?alt=media&token=a33a17ba-5184-4014-9ff7-f316e38a9a3f')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod26', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F1%23%23lZd6f%2FImage?alt=media&token=f1de8eb2-8716-4ecd-ac2f-28dbba72947e')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod26', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F1%23%23lZd6f%2FImage_8?alt=media&token=ddb29c83-1157-415b-9ab5-ac902e22d78e')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod27', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F5%23%23nZd6o?alt=media&token=f9e9ff46-3f14-4c75-ac2f-7800880d0b37')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod27', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F5%23%23nZd6o_2?alt=media&token=f404b7c4-e163-49d2-be7d-07243b1839eb')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod28', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F7%23%23pZd6t?alt=media&token=dfe1f6c7-2237-4be4-8c19-ce087431557a')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod28', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F7%23%23pZd6t_100?alt=media&token=39a41ff2-0dfa-4e3e-b240-78dde6b589e0')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod29', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F3%23%23yZd6i?alt=media&token=2ad8217b-8cb3-4bb4-91fe-c2d57a43a98d')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod29', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F3%23%23yZd6i_132?alt=media&token=a844be32-1637-45ff-b1fb-9192236fe739')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod29', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F3%23%23yZd6i_136?alt=media&token=9211c3a4-fb4d-4f0b-bf6f-3cf2cdc5891f')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod30', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F5%23%23%3DZd6n?alt=media&token=81a4b9a5-72fa-40f4-9d78-c6d0c2abd7eb')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod30', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F5%23%23%3DZd6n_156?alt=media&token=2e58ae3b-d89c-45c8-aa5f-4ddae7b5c850')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod30', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F5%23%23%3DZd6n_168?alt=media&token=c89d0ed9-dea3-4766-a94f-52618c833d92')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod30', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F5%23%23%3DZd6n_84?alt=media&token=1b2d9aec-9879-454e-8220-a96ba2493e4d')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod31', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F8%23%23vZd6w?alt=media&token=8bfe4987-2b3d-4246-b468-16aef477337f')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod31', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F8%23%23vZd6w_136?alt=media&token=b4c2e889-a33e-47d4-b44e-d9ad2cef71a3')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod31', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F8%23%23vZd6w_174?alt=media&token=923b8d50-f91a-4ce5-9d9c-27d50be3c0cb')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod32', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F9%23%23xZd6y%2FImage?alt=media&token=32954241-9590-477f-824d-f22e296d0f3a')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod32', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F9%23%23xZd6y%2FImage_12?alt=media&token=516771aa-dd8f-4248-a6e7-1e95a6b32ceb')
GO
INSERT [dbo].[ImageProduct] ([IdProduct], [Source]) VALUES (N'prod32', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F9%23%23xZd6y%2FImage_136?alt=media&token=6ccefe3b-9180-4f05-af5f-180942146512')
GO


INSERT [dbo].[Promo] ([Id], [IdShop], [Code], [Name], [Description], [DateBegin], [DateEnd], [Amount], [AmountUsed], [MaxSale], [MinCost], [CustomerType], [Sale], [Status]) VALUES (N'prm25', N'usr025', N'QWIRT12', N'Celebrate', N'Celebrate our anniversay', CAST(N'2022-12-31T12:00:00' AS SmallDateTime), CAST(N'2023-03-30T12:00:00' AS SmallDateTime), -1, 0, 1.7976931348623157E+308, 1, 0, 5, 1)
GO
INSERT [dbo].[Promo] ([Id], [IdShop], [Code], [Name], [Description], [DateBegin], [DateEnd], [Amount], [AmountUsed], [MaxSale], [MinCost], [CustomerType], [Sale], [Status]) VALUES (N'prm26', N'usr026', N'QUOW23', N'Festival', N'Festival season, sale up to 10%', CAST(N'2022-12-31T12:00:00' AS SmallDateTime), CAST(N'2023-03-28T12:00:00' AS SmallDateTime), 10, 0, 1.7976931348623157E+308, 0, 0, 10, 1)
GO
INSERT [dbo].[Promo] ([Id], [IdShop], [Code], [Name], [Description], [DateBegin], [DateEnd], [Amount], [AmountUsed], [MaxSale], [MinCost], [CustomerType], [Sale], [Status]) VALUES (N'prm27', N'usr025', N'QWERT11', N'New Year Sale', N'New year sale off up to 20%', CAST(N'2022-12-31T12:00:00' AS SmallDateTime), CAST(N'2023-03-27T12:00:00' AS SmallDateTime), -1, 0, 1.7976931348623157E+308, 2, 0, 10, 1)
GO
INSERT [dbo].[Promo] ([Id], [IdShop], [Code], [Name], [Description], [DateBegin], [DateEnd], [Amount], [AmountUsed], [MaxSale], [MinCost], [CustomerType], [Sale], [Status]) VALUES (N'prm28', N'usr026', N'QUAO123', N'Happy New Year', N'New year sale', CAST(N'2022-12-31T12:00:00' AS SmallDateTime), CAST(N'2023-03-31T12:00:00' AS SmallDateTime), -1, 0, 1.7976931348623157E+308, 1, 0, 8, 1)
GO
INSERT [dbo].[Promo] ([Id], [IdShop], [Code], [Name], [Description], [DateBegin], [DateEnd], [Amount], [AmountUsed], [MaxSale], [MinCost], [CustomerType], [Sale], [Status]) VALUES (N'prm29', N'usr025', N'QWART16', N'School day sale off', N'Sale up to 20%', CAST(N'2022-12-31T12:00:00' AS SmallDateTime), CAST(N'2023-03-28T12:00:00' AS SmallDateTime), 20, 0, 1.7976931348623157E+308, 1, 0, 20, 0)
GO
INSERT [dbo].[Promo] ([Id], [IdShop], [Code], [Name], [Description], [DateBegin], [DateEnd], [Amount], [AmountUsed], [MaxSale], [MinCost], [CustomerType], [Sale], [Status]) VALUES (N'prm30', N'usr026', N'QUIRT65', N'Summer sale', N'Summer sale up to 5%', CAST(N'2022-12-31T12:00:00' AS SmallDateTime), CAST(N'2023-03-28T12:00:00' AS SmallDateTime), 10, 0, 1.7976931348623157E+308, 1, 0, 5, 0)
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod25', N'prm25')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod25', N'prm29')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod26', N'prm25')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod26', N'prm29')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod27', N'prm25')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod27', N'prm29')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod28', N'prm25')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod28', N'prm27')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod29', N'prm25')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod29', N'prm27')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod30', N'prm25')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod31', N'prm25')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod32', N'prm25')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod32', N'prm27')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod33', N'prm25')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod35', N'prm26')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod36', N'prm28')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod36', N'prm30')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod37', N'prm26')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod38', N'prm28')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod39', N'prm25')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod39', N'prm27')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod40', N'prm25')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod40', N'prm27')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod41', N'prm28')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod41', N'prm30')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod42', N'prm28')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod42', N'prm30')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod43', N'prm25')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod44', N'prm26')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod44', N'prm30')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod45', N'prm30')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod46', N'prm30')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod48', N'prm28')
GO
INSERT [dbo].[PromoDetail] ([IdProduct], [IdPromo]) VALUES (N'prod48', N'prm30')


INSERT [dbo].[MOrder] ([Id], [IdCustomer], [IdShop], [ShipTotal], [DateBegin], [DateEnd], [OrderTotal], [Discounted], [Promo], [ShippingMethod], [ShippingSpeedMethod], [AddressIndex], [Status]) VALUES (N'ord40', N'usr026', N'usr025', 0, CAST(N'2023-01-17T17:25:00' AS SmallDateTime), CAST(N'2023-01-17T17:29:00' AS SmallDateTime), 498, NULL, N'prm25', 1, 0, N'add25', N'Completed')
GO
INSERT [dbo].[MOrder] ([Id], [IdCustomer], [IdShop], [ShipTotal], [DateBegin], [DateEnd], [OrderTotal], [Discounted], [Promo], [ShippingMethod], [ShippingSpeedMethod], [AddressIndex], [Status]) VALUES (N'ord41', N'usr026', N'usr025', 0, CAST(N'2023-01-17T17:27:00' AS SmallDateTime), CAST(N'2023-01-17T17:28:00' AS SmallDateTime), 538, NULL, N'prm25', 2, 0, N'add25', N'Completed')
GO
INSERT [dbo].[MOrder] ([Id], [IdCustomer], [IdShop], [ShipTotal], [DateBegin], [DateEnd], [OrderTotal], [Discounted], [Promo], [ShippingMethod], [ShippingSpeedMethod], [AddressIndex], [Status]) VALUES (N'ord43', N'usr025', N'usr026', 0, CAST(N'2023-01-17T17:20:00' AS SmallDateTime), CAST(N'2023-01-17T17:23:00' AS SmallDateTime), 48, NULL, N'prm28', 1, 0, N'add26', N'Completed')
GO
INSERT [dbo].[MOrder] ([Id], [IdCustomer], [IdShop], [ShipTotal], [DateBegin], [DateEnd], [OrderTotal], [Discounted], [Promo], [ShippingMethod], [ShippingSpeedMethod], [AddressIndex], [Status]) VALUES (N'ord48', N'usr025', N'usr026', 0, CAST(N'2023-01-17T17:21:00' AS SmallDateTime), CAST(N'2023-01-17T17:23:00' AS SmallDateTime), 81, NULL, NULL, 2, 0, N'add26', N'Completed')
GO
INSERT [dbo].[MOrder] ([Id], [IdCustomer], [IdShop], [ShipTotal], [DateBegin], [DateEnd], [OrderTotal], [Discounted], [Promo], [ShippingMethod], [ShippingSpeedMethod], [AddressIndex], [Status]) VALUES (N'ord49', N'usr025', N'usr026', 0, CAST(N'2023-01-17T17:22:00' AS SmallDateTime), CAST(N'2023-01-17T17:23:00' AS SmallDateTime), 269, NULL, N'prm28', 1, 0, N'add26', N'Completed')
GO


INSERT [dbo].[Rating] ([Id], [DateRating], [Rating], [Comment]) VALUES (N'rate25', CAST(N'2023-01-17T17:41:00' AS SmallDateTime), 4, N'Relly good though')
GO
INSERT [dbo].[Rating] ([Id], [DateRating], [Rating], [Comment]) VALUES (N'rate26', CAST(N'2023-01-17T17:41:00' AS SmallDateTime), 5, N'It absolutely waterproof')
GO
INSERT [dbo].[Rating] ([Id], [DateRating], [Rating], [Comment]) VALUES (N'rate27', CAST(N'2023-01-17T17:30:00' AS SmallDateTime), 5, N'Beautiful corlor')
GO
INSERT [dbo].[Rating] ([Id], [DateRating], [Rating], [Comment]) VALUES (N'rate28', CAST(N'2023-01-17T17:30:00' AS SmallDateTime), 3, N'A bit disappointed')
GO
INSERT [dbo].[Rating] ([Id], [DateRating], [Rating], [Comment]) VALUES (N'rate29', CAST(N'2023-01-17T17:42:00' AS SmallDateTime), 4, N'A bit to heavy for me')
GO
INSERT [dbo].[Rating] ([Id], [DateRating], [Rating], [Comment]) VALUES (N'rate30', CAST(N'2023-01-17T17:29:00' AS SmallDateTime), 5, N'So so')
GO
INSERT [dbo].[Rating] ([Id], [DateRating], [Rating], [Comment]) VALUES (N'rate31', CAST(N'2023-01-17T17:29:00' AS SmallDateTime), 3, N'Too small for me')
GO
INSERT [dbo].[Rating] ([Id], [DateRating], [Rating], [Comment]) VALUES (N'rate32', CAST(N'2023-01-17T17:41:00' AS SmallDateTime), 5, N'Nice, recommend it for this winter')
GO
INSERT [dbo].[Rating] ([Id], [DateRating], [Rating], [Comment]) VALUES (N'rate33', CAST(N'2023-01-17T17:42:00' AS SmallDateTime), 4, N'Its too short for me
Im kind of sad receiving this')
GO
INSERT [dbo].[Rating] ([Id], [DateRating], [Rating], [Comment]) VALUES (N'rate34', CAST(N'2023-01-17T17:29:00' AS SmallDateTime), 5, N'Worth it!')
GO
INSERT [dbo].[RatingInfo] ([IdUser], [IdRating], [Comment], [DateReply]) VALUES (N'usr025', N'rate25', N'We hope to serve you in the future', CAST(N'2023-01-17T17:44:17.783' AS DateTime))
GO
INSERT [dbo].[RatingInfo] ([IdUser], [IdRating], [Comment], [DateReply]) VALUES (N'usr025', N'rate26', N'Thank you for buying', CAST(N'2023-01-17T17:45:56.957' AS DateTime))
GO
INSERT [dbo].[RatingInfo] ([IdUser], [IdRating], [Comment], [DateReply]) VALUES (N'usr025', N'rate29', N'We''re sorry for that, you can contact us directly for size detail
Thank you for buying', CAST(N'2023-01-17T17:45:39.073' AS DateTime))
GO
INSERT [dbo].[RatingInfo] ([IdUser], [IdRating], [Comment], [DateReply]) VALUES (N'usr025', N'rate32', N'Thank you so much', CAST(N'2023-01-17T17:43:43.127' AS DateTime))
GO
INSERT [dbo].[RatingInfo] ([IdUser], [IdRating], [Comment], [DateReply]) VALUES (N'usr026', N'rate27', N'Thank you for purchasing', CAST(N'2023-01-17T17:33:30.090' AS DateTime))
GO
INSERT [dbo].[RatingInfo] ([IdUser], [IdRating], [Comment], [DateReply]) VALUES (N'usr026', N'rate30', N'Thank you for rating', CAST(N'2023-01-17T17:36:42.703' AS DateTime))
GO
INSERT [dbo].[RatingInfo] ([IdUser], [IdRating], [Comment], [DateReply]) VALUES (N'usr026', N'rate31', N'We sorry for that, hope to serve you again', CAST(N'2023-01-17T17:33:08.353' AS DateTime))
GO
INSERT [dbo].[RatingInfo] ([IdUser], [IdRating], [Comment], [DateReply]) VALUES (N'usr026', N'rate34', N'Thank you for your rating', CAST(N'2023-01-17T17:30:37.220' AS DateTime))
GO
INSERT [dbo].[OrderInfo] ([IdOrder], [IdProduct], [IdRating], [ImageProduct], [Size], [Count], [TotalPrice]) VALUES (N'ord40', N'prod39', N'rate32', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F5%23%23UZd6o?alt=media&token=c33cc093-128e-4d80-99da-55620cf09a1f', N'S', 1, 498)
GO
INSERT [dbo].[OrderInfo] ([IdOrder], [IdProduct], [IdRating], [ImageProduct], [Size], [Count], [TotalPrice]) VALUES (N'ord40', N'prod40', N'rate25', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F4%23%23SZd6k?alt=media&token=97c3908f-f6e0-4314-a915-6c00c2d55913', N'M', 1, 498)
GO
INSERT [dbo].[OrderInfo] ([IdOrder], [IdProduct], [IdRating], [ImageProduct], [Size], [Count], [TotalPrice]) VALUES (N'ord40', N'prod43', N'rate26', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F2%23%23QZd6g?alt=media&token=287c1e37-6124-4be2-abf9-c16745a566ea', N'S', 1, 498)
GO
INSERT [dbo].[OrderInfo] ([IdOrder], [IdProduct], [IdRating], [ImageProduct], [Size], [Count], [TotalPrice]) VALUES (N'ord41', N'prod31', N'rate33', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F8%23%23vZd6w?alt=media&token=8bfe4987-2b3d-4246-b468-16aef477337f', N'L', 1, 538)
GO
INSERT [dbo].[OrderInfo] ([IdOrder], [IdProduct], [IdRating], [ImageProduct], [Size], [Count], [TotalPrice]) VALUES (N'ord41', N'prod32', N'rate29', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F9%23%23xZd6y%2FImage?alt=media&token=32954241-9590-477f-824d-f22e296d0f3a', N'S', 1, 538)
GO
INSERT [dbo].[OrderInfo] ([IdOrder], [IdProduct], [IdRating], [ImageProduct], [Size], [Count], [TotalPrice]) VALUES (N'ord43', N'prod36', N'rate34', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F8%23%23kZd6x?alt=media&token=4a7b2191-5a59-4c4b-b95b-cb24d054c8d5', N'L', 1, 48)
GO
INSERT [dbo].[OrderInfo] ([IdOrder], [IdProduct], [IdRating], [ImageProduct], [Size], [Count], [TotalPrice]) VALUES (N'ord48', N'prod34', N'rate31', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F9%23%23aZd6y?alt=media&token=48a93448-ed87-475c-a6e7-8b8526710ee9', N'S', 1, 86)
GO
INSERT [dbo].[OrderInfo] ([IdOrder], [IdProduct], [IdRating], [ImageProduct], [Size], [Count], [TotalPrice]) VALUES (N'ord48', N'prod46', N'rate30', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F1%23%23iZd6d?alt=media&token=53bc3bf2-ab17-471f-a402-32b7465d2b6a', N'M', 1, 86)
GO
INSERT [dbo].[OrderInfo] ([IdOrder], [IdProduct], [IdRating], [ImageProduct], [Size], [Count], [TotalPrice]) VALUES (N'ord49', N'prod41', N'rate27', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F3%23%23lZd6i?alt=media&token=c780d18a-f3ba-4b9e-af29-60c8145e9f12', N'S', 1, 269)
GO
INSERT [dbo].[OrderInfo] ([IdOrder], [IdProduct], [IdRating], [ImageProduct], [Size], [Count], [TotalPrice]) VALUES (N'ord49', N'prod48', N'rate28', N'https://firebasestorage.googleapis.com/v0/b/wano-wpf.appspot.com/o/Product%2F0%23%23cZd6b_10?alt=media&token=843b1007-b9a4-49e8-9de8-5ac846e17138', N'L', 1, 269)
GO

--Check Data
select * from MUser
select * from UserLogin
select * from Rating
select * from MOrder
select * from Product
select * from OrderInfo
select * from dbo.Notification
select * from dbo.ImageProduct