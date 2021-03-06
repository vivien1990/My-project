﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using Common;
public class ui_register : basePanel
{
    int tab = 0;
    string txt_name;
    string txt_codenum;
    string txt_phnoe;
    string txt_email;
    string txt_passward;

    public ui_register(Transform go)
    {
        m_panel = go;

        m_panel.FindChild("tab_phone").GetComponent<Button>().onClick.AddListener(on_tab_phone);
        m_panel.FindChild("tab_email").GetComponent<Button>().onClick.AddListener(on_tab_email);

        m_panel.FindChild("return").GetComponent<Button>().onClick.AddListener(on_return);
        m_panel.FindChild("register").GetComponent<Button>().onClick.AddListener(on_register);

        m_panel.FindChild("panel_phone/codenum/get_num").GetComponent<Button>().onClick.AddListener(on_get_num);
        m_panel.FindChild("panel_mail/codenum/get_num").GetComponent<Button>().onClick.AddListener(on_get_num);

        Dropdown drop = m_panel.FindChild("panel_phone/Dropdown").GetComponent<Dropdown>();

        on_tab_phone();
    }

    void on_return()
    {
        hide();
        testtool.panel_login.show();
    }

    void on_get_num()
    {
        if (tab == 0)
        {
            txt_phnoe = m_panel.FindChild("panel_phone/phonenum").GetComponent<InputField>().text;
            if (check_phone(txt_phnoe))
            {
                txt_phnoe += getphone_group();
                api_tool._instance.validPhone(txt_phnoe, (bool timeout, WWW www) => { api_tool._instance.getPhoneCode(txt_phnoe, on_get_phone_code); });
            }
        }
        else
        {
            txt_email = m_panel.FindChild("panel_mail/email").GetComponent<InputField>().text;
            if (check_email(txt_email))
                api_tool._instance.validEmail(txt_email, (bool timeout, WWW www) => { api_tool._instance.getEmailCode(txt_email, "cn", on_get_email_code); });
        }
    }

    public string getphone_group()
    {
        Dropdown drop = m_panel.FindChild("panel_phone/Dropdown").GetComponent<Dropdown>();
        string txt="";
        Debug.Log(drop.itemText.text);
        switch (drop.itemText.text)
        {
            case "中国+86": txt = "@86"; break;
        }
        return txt;
    }

    public bool check_phone(string txt)
    {
        if (txt == "")
        {
            testtool.showNotice("请输入手机号");
            return false;
        }
        return true;
    }

    public bool check_email(string txt)
    {
        if (txt == "")
        {
            testtool.showNotice("请输入邮箱");
            return false;
        }

        string expression = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
        if (!System.Text.RegularExpressions.Regex.IsMatch(txt, expression))
        {
            testtool.showNotice("邮箱格式错误");
            return false;
        }

        return true;
    }

    void on_register()
    {
       
        string txt_passward1;
        string txt_passward2;
        if (tab == 0)
        {
            txt_name = m_panel.FindChild("panel_phone/userid").GetComponent<InputField>().text;
            txt_phnoe = m_panel.FindChild("panel_phone/phonenum").GetComponent<InputField>().text;
            txt_codenum = m_panel.FindChild("panel_phone/codenum").GetComponent<InputField>().text;
            txt_passward1 = m_panel.FindChild("panel_phone/passward1").GetComponent<InputField>().text;
            txt_passward2 = m_panel.FindChild("panel_phone/passward2").GetComponent<InputField>().text;

            if (!check_phone(txt_phnoe))
                return;
            txt_phnoe = txt_phnoe + getphone_group();
        }
        else
        {
            txt_name = m_panel.FindChild("panel_mail/userid").GetComponent<InputField>().text;
            txt_email = m_panel.FindChild("panel_mail/email").GetComponent<InputField>().text;
            txt_codenum = m_panel.FindChild("panel_mail/codenum").GetComponent<InputField>().text;
            txt_passward1 = m_panel.FindChild("panel_mail/passward1").GetComponent<InputField>().text;
            txt_passward2 = m_panel.FindChild("panel_mail/passward2").GetComponent<InputField>().text;

            if (!check_email(txt_email))
                return; 
        }
        if (txt_codenum == "")
        {
            testtool.showNotice("请输入验证码");
            return;
        }
        if (txt_name == "")
        {
            testtool.showNotice("请输入用户名");
            return;
        }
        if (txt_passward1 == "" || txt_passward2 == "")
        {
            testtool.showNotice("请输入密码");
            return;
        }

        if (txt_passward1 != txt_passward2)
        {
            testtool.showNotice("密码不一致");
            return;
        }
        txt_passward = txt_passward1;

        api_tool._instance.validUid(txt_name, (bool timeout, WWW www) => {
            if (tab == 0)
            {
                 api_tool._instance.validPhone(txt_phnoe,
                     (bool timeout1, WWW www1) => { api_tool._instance.registerByPhone(txt_phnoe, txt_codenum, txt_passward, "CN", txt_name, on_register); });
            }
            else
            {
                api_tool._instance.validEmail(txt_email,
                     (bool timeout1, WWW www1) => { api_tool._instance.registerByEmail(txt_email, txt_codenum, txt_passward, "CN", txt_name, on_register); });
            }

        });
    }

    void on_tab_phone()
    {
        tab = 0;
        m_panel.FindChild("tab_phone").GetComponent<Button>().interactable = false;
        m_panel.FindChild("tab_email").GetComponent<Button>().interactable = true;
        m_panel.FindChild("panel_phone").gameObject.SetActive(true);
        m_panel.FindChild("panel_mail").gameObject.SetActive(false);
    }

    void on_tab_email()
    {
        tab = 1;
        m_panel.FindChild("tab_phone").GetComponent<Button>().interactable = true;
        m_panel.FindChild("tab_email").GetComponent<Button>().interactable = false;
        m_panel.FindChild("panel_phone").gameObject.SetActive(false);
        m_panel.FindChild("panel_mail").gameObject.SetActive(true);
    }   

    private void on_get_phone_code(bool timeout, WWW www)       
    {

    }

    private void on_get_email_code(bool timeout, WWW www)
    {

    }

    private void on_register_byPhone(bool timeout, WWW www)
    {

    }

    private void on_register(bool timeout, WWW www)
    {
        string data = MyJson.Parse(www.text).AsDict()["data"].ToString();
        PlayerPrefs.SetString(roleInfo.getInstance().key_info, data);
        roleInfo.getInstance().get_info();

        if (roleInfo.getInstance().wallet == "")
        {
            hide();
            testtool.panel_bindwallet.show();
        }
        else
        {

        }
    }
}
