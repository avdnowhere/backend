## STEP BY STEP

**1. You need to setup a new application on Internet Information Services (IIS):**

### `Open IIS > Sites > Right Click on Default Web Site > Click on Add Application`

On **Alias**, put **CarsomeApi** and browse **Physical path** to **WebAPI** folder, then click **OK**.<br/><br/>

**2. You might want to add permission to WebAPI folder:**

Right click on **WebAPI** folder > select **Security** tab.

Click **Edit** button > click **Add** button.

Click **Advanced** button > click **Find Now** button.

Select **Everyone** from the Search results > click **OK** > then click **OK** again.

On the Permission box, tick a checkbox for **Full control** on column **Allow**.

Click **OK** > then click **OK** again.<br/><br/>

**3. For the Database:**

I use **MySQL** for this application and you need to create a new **Database** called **carsome**.
Then run this **SQL Query** to create necessary tables:

**CREATE TABLE IF NOT EXISTS `carsome_bookdetailstbl` (
  `BookDetailsId` bigint(20) NOT NULL AUTO_INCREMENT,
  `Date` date NOT NULL DEFAULT '1900-01-01',
  `BookTimeId` bigint(20) NOT NULL,
  `Name` varchar(500) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `MobileNo` varchar(50) NOT NULL,
  `CarRegistrationNo` varchar(50) NOT NULL,
  `BookingNo` varchar(50) NOT NULL,
  `CreatedDate` datetime NOT NULL DEFAULT '1900-01-01 00:00:00',
  `LastUpdatedDate` datetime NOT NULL DEFAULT '1900-01-01 00:00:00',
  PRIMARY KEY (`BookDetailsId`),
  KEY `FK_carsome_bookdetailstbl_carsome_booktimetbl` (`BookTimeId`),
  CONSTRAINT `FK_carsome_bookdetailstbl_carsome_booktimetbl` FOREIGN KEY (`BookTimeId`) REFERENCES `carsome_booktimetbl` (`BookTimeId`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=42 DEFAULT CHARSET=utf8;**

**CREATE TABLE IF NOT EXISTS `carsome_bookrunningtbl` (
  `BookRunningId` bigint(20) NOT NULL AUTO_INCREMENT,
  `Code` varchar(50) NOT NULL,
  `RunningNo` int(10) NOT NULL,
  `CreatedDate` datetime NOT NULL DEFAULT '1900-01-01 00:00:00',
  `LastUpdatedDate` datetime NOT NULL DEFAULT '1900-01-01 00:00:00',
  PRIMARY KEY (`BookRunningId`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;**

**CREATE TABLE IF NOT EXISTS `carsome_booktimetbl` (
  `BookTimeId` bigint(20) NOT NULL AUTO_INCREMENT,
  `Code` varchar(50) NOT NULL,
  `Name` varchar(100) NOT NULL,
  `CreatedDate` datetime NOT NULL DEFAULT '1900-01-01 00:00:00',
  `LastUpdatedDate` datetime NOT NULL DEFAULT '1900-01-01 00:00:00',
  PRIMARY KEY (`BookTimeId`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8;**

Then, you need to setup your connection string on **WebAPI\App_Start\WebApiConfig.cs**:

### string conn_string = "server=localhost;port=3306;database=carsome;username=(your_username);password=(your_password);";<br/><br/>

**4. The backend code located at:**

### `WebAPI\Controllers\CarsomeController.cs`<br/><br/>

**5. You need the frontend code to better integration with the application, click this link for more details:**

### `https://github.com/avdnowhere/frontend/tree/dev`
