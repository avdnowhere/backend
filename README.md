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

**3. The backend code located at:**

### `CarsomeController.cs`
