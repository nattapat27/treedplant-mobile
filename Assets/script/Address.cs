using System.Collections;
using System.Collections.Generic;


public class Address 
{
    private string _detail;
    private string _district;
    private string _province;
    private string _zipcode;

    public string Zipcode
    {
        get
        {
            return this._zipcode;
        }
        set
        {
            this._zipcode = value;
        }
    }
    public string Province
    {
        get
        {
            return this._province;
        }
        set
        {
            this._province = value;
        }
    }

    public string District
    {
        get
        {
            return this._district;
        }
        set
        {
            this._district = value;
        }
    }
    public string Detail
    {
        get
        {
            return this._detail;
        }
        set
        {
            this._detail = value;
        }
    }


}
