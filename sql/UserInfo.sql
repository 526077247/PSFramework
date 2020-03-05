CREATE TABLE `UserInfo` (
`Id` varchar(50) not null,
`Name` varchar(50) ,
`NickName` varchar(50) ,
`Tel` varchar(50) ,
`Mail` varchar(50) ,
`Psw` varchar(50) ,
`Status` int,
`CreateTime` datetime,
INDEX `IX_UserInfo_id` (`Id`),
INDEX `IX_UserInfo_CreateTime` (`CreateTime`),
PRIMARY KEY (`Id`)
);
