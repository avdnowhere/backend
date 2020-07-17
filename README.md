## STEP BY STEP

**1. After Clone or Download the repository, open the project on Microsoft Visual Studio:**

Then click on **Build** menu > click **Clean Solution** option.

After that, click on **Build** menu again > click **Rebuild Solution** option.

NB: If you are having a problem with the solution, then it could be because of missing **MySql.Data.dll** file. You can download and install the dll file from https://dev.mysql.com/downloads/connector/net/. Then you need to import it as a **References** into the solution. Kindly follow the steps on [this video](https://www.youtube.com/embed/TcovfE8IsHs?start=415&end=507) for more details.<br/><br/>

**2. You need to setup a new application on Internet Information Services (IIS):**

### `Open IIS > Sites > Right Click on Default Web Site > Click on Add Application`

On **Alias**, put **CarsomeApi** and browse **Physical path** to **WebAPI** folder, then click **OK**.<br/><br/>

**3. You might want to add permission to WebAPI folder:**

Right click on **WebAPI** folder > select **Security** tab.

Click **Edit** button > click **Add** button.

Click **Advanced** button > click **Find Now** button.

Select **Everyone** from the **Search results** > click **OK** > then click **OK** again.

On the **Permissions** box, tick a checkbox for **Full control** on column **Allow**.

Click **OK** > then click **OK** again.

NB: If there is still an issue, you can perform **Clean Solution** and **Rebuild Solution** again.<br/><br/>

**4. For the Database:**

I use **MySQL** for this application and you need to create a new **Database** called **carsome**.
Then run these **SQL Queries** to create necessary tables:

**CREATE TABLE IF NOT EXISTS `carsome_bookrunningtbl` (
  `BookRunningId` bigint(20) NOT NULL AUTO_INCREMENT,
  `Code` varchar(50) NOT NULL,
  `RunningNo` int(10) NOT NULL,
  `CreatedDate` datetime NOT NULL DEFAULT '1900-01-01 00:00:00',
  `LastUpdatedDate` datetime NOT NULL DEFAULT '1900-01-01 00:00:00',
  PRIMARY KEY (`BookRunningId`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;**

**INSERT INTO `carsome_bookrunningtbl` (`BookRunningId`, `Code`, `RunningNo`, `CreatedDate`, `LastUpdatedDate`) VALUES
	(1, 'BookingNo', 1, '2020-06-22 00:00:00', '2020-06-22 00:00:00');**

<br/>

**CREATE TABLE IF NOT EXISTS `carsome_booktimetbl` (
  `BookTimeId` bigint(20) NOT NULL AUTO_INCREMENT,
  `Code` varchar(50) NOT NULL,
  `Name` varchar(100) NOT NULL,
  `CreatedDate` datetime NOT NULL DEFAULT '1900-01-01 00:00:00',
  `LastUpdatedDate` datetime NOT NULL DEFAULT '1900-01-01 00:00:00',
  PRIMARY KEY (`BookTimeId`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8;**

**INSERT INTO `carsome_booktimetbl` (`BookTimeId`, `Code`, `Name`, `CreatedDate`, `LastUpdatedDate`) VALUES
	(1, '09.00 AM', '09.00', '2020-06-22 00:00:00', '2020-06-22 00:00:00'),
	(2, '09.30 AM', '09.30', '2020-06-22 00:00:00', '2020-06-22 00:00:00'),
	(3, '10.00 AM', '10.00', '2020-06-22 00:00:00', '2020-06-22 00:00:00'),
	(4, '10.30 AM', '10.30', '2020-06-22 00:00:00', '2020-06-22 00:00:00'),
	(5, '11.00 AM', '11.00', '2020-06-22 00:00:00', '2020-06-22 00:00:00'),
	(6, '11.30 AM', '11.30', '2020-06-22 00:00:00', '2020-06-22 00:00:00'),
	(7, '12.00 PM', '12.00', '2020-06-22 00:00:00', '2020-06-22 00:00:00'),
	(8, '12.30 PM', '12.30', '2020-06-22 00:00:00', '2020-06-22 00:00:00'),
	(9, '01.00 PM', '13.00', '2020-06-22 00:00:00', '2020-06-22 00:00:00'),
	(10, '01.30 PM', '13.30', '2020-06-22 00:00:00', '2020-06-22 00:00:00'),
	(11, '02.00 PM', '14.00', '2020-06-22 00:00:00', '2020-06-22 00:00:00'),
	(12, '02.30 PM', '14.30', '2020-06-22 00:00:00', '2020-06-22 00:00:00'),
	(13, '03.00 PM', '15.00', '2020-06-22 00:00:00', '2020-06-22 00:00:00'),
	(14, '03.30 PM', '15.30', '2020-06-22 00:00:00', '2020-06-22 00:00:00'),
	(15, '04.00 PM', '16.00', '2020-06-22 00:00:00', '2020-06-22 00:00:00'),
	(16, '04.30 PM', '16.30', '2020-06-22 00:00:00', '2020-06-22 00:00:00'),
	(17, '05.00 PM', '17.00', '2020-06-22 00:00:00', '2020-06-22 00:00:00'),
	(18, '05.30 PM', '17.30', '2020-06-22 00:00:00', '2020-06-22 00:00:00'),
	(19, '06.00 PM', '18.00', '2020-06-22 00:00:00', '2020-06-22 00:00:00');**

<br/>

**CREATE TABLE IF NOT EXISTS `carsome_bookstatustbl` (
  `BookStatusId` bigint(20) NOT NULL AUTO_INCREMENT,
  `Code` varchar(50) NOT NULL,
  `Name` varchar(100) NOT NULL,
  `CreatedDate` datetime NOT NULL DEFAULT '1900-01-01 00:00:00',
  `LastUpdatedDate` datetime NOT NULL DEFAULT '1900-01-01 00:00:00',
  PRIMARY KEY (`BookStatusId`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;**

**INSERT INTO `carsome_bookstatustbl` (`BookStatusId`, `Code`, `Name`, `CreatedDate`, `LastUpdatedDate`) VALUES
	(1, 'Confirmed', 'Confirmed', '2020-06-22 00:00:00', '2020-06-22 00:00:00'),
	(2, 'Cancelled', 'Cancelled', '2020-06-22 00:00:00', '2020-06-22 00:00:00'),
	(3, 'Rescheduled', 'Rescheduled', '2020-06-22 00:00:00', '2020-06-22 00:00:00');**

<br/>

**CREATE TABLE IF NOT EXISTS `carsome_bookdetailstbl` (
  `BookDetailsId` bigint(20) NOT NULL AUTO_INCREMENT,
  `Date` date NOT NULL DEFAULT '1900-01-01',
  `BookTimeId` bigint(20) NOT NULL,
  `Name` varchar(500) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `MobileNo` varchar(50) NOT NULL,
  `CarRegistrationNo` varchar(50) NOT NULL,
  `BookingNo` varchar(50) NOT NULL,
  `BookStatusId` bigint(20) NOT NULL,
  `CreatedDate` datetime NOT NULL DEFAULT '1900-01-01 00:00:00',
  `LastUpdatedDate` datetime NOT NULL DEFAULT '1900-01-01 00:00:00',
  PRIMARY KEY (`BookDetailsId`),
  KEY `FK_carsome_bookdetailstbl_carsome_booktimetbl` (`BookTimeId`),
  KEY `FK_carsome_bookdetailstbl_carsome_bookstatustbl` (`BookStatusId`),
  CONSTRAINT `FK_carsome_bookdetailstbl_carsome_bookstatustbl` FOREIGN KEY (`BookStatusId`) REFERENCES `carsome_bookstatustbl` (`BookStatusId`) ON DELETE CASCADE,
  CONSTRAINT `FK_carsome_bookdetailstbl_carsome_booktimetbl` FOREIGN KEY (`BookTimeId`) REFERENCES `carsome_booktimetbl` (`BookTimeId`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=53 DEFAULT CHARSET=utf8;**

<br/>

Then, you need to setup your connection string on **WebAPI\App_Start\WebApiConfig.cs**:

**string conn_string = "server=localhost;port=3306;database=carsome;username=(your_username);password=(your_password);";**

NB: You need to **Build** the solution again every time you made any changes (Press **Ctrl+Shift+B**).<br/><br/>

**5. The backend code located at:**

### `WebAPI\Controllers\CarsomeController.cs`<br/><br/>

**6. You need the frontend code to better integration with the application, click [frontend-code-link](https://github.com/avdnowhere/frontend/tree/dev) for more details.**
