using System.Collections;
using System.Collections.Generic;

public class User
{
    private string _id;
    private string _name;
    private string _email;
    private string _phone;
    private string _password;
    public string Id
    {
        get
        {
            return this._id;
        }
        set
        {
            this._id = value;
        }
    }
    public string Passwordd
    {
        get
        {
            return this._password;
        }
        set
        {
            this._password = value;
        }
    }
    public string Name
    {
        get
        {
            return this._name;
        }
        set
        {
            this._name = value;
        }
    }
    public string Email
    {
        get
        {
            return this._email;
        }
        set
        {
            this._email = value;
        }
    }
    public string Phone
    {
        get
        {
            return this._phone;
        }
        set
        {
            this._phone = value;
        }
    }
}
