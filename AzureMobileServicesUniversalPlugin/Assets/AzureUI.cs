#region License
// Copyright (c) 2014 Bit Rave Pty Ltd
//
// 1. OWNERSHIP, LICENSE GRANT
// Subject to the terms below (the 'License Agreement'), Bit Rave Pty Ltd ('We', 'Us') 
// grants you to install and use Azure Mobile Services for Unity (the 'Software').
//
// This is a license agreement and not an agreement for sale. We reserve ownership 
// of all intellectual property rights inherent in or relating to the Software, 
// which include, but are not limited to, all copyright, patent rights, all rights 
// in relation to registered and unregistered trademarks (including service marks), 
// confidential information (including trade secrets and know-how) and all rights 
// other than those expressly granted by this Agreement.
//
// Subject to the terms and conditions of this License Agreement, We grant to You 
// a non-transferable, non-exclusive license for a Designated User (as defined below) 
// within Your organization to install and use the Software on any workstations used 
// exclusively by such Designated User and for You to distribute the Software as part 
// of your Unity applications or games, solely in connection with distribution of 
// the Software in accordance with sections 3 and 4 below. This license is not 
// sublicensable except as explicitly set forth herein. "Designated User(s)" shall 
// mean Your employee(s) acting within the scope of their employment or Your consultant(s) 
// or contractor(s) acting within the scope of the services they provide for You or on Your behalf.

// 2. PERMITTED USES, SOURCE CODE, MODIFICATIONS
// We provide You with source code so that You can create Modifications of the original Software, 
// where Modification means: a) any addition to or deletion from the contents of a file included 
// in the original Software or previous Modifications created by You, or b) any new file that 
// contains any part of the original Software or previous Modifications. While You retain all 
// rights to any original work authored by You as part of the Modifications, We continue to own 
// all copyright and other intellectual property rights in the Software.

// 3. DISTRIBUTION
// You may distribute the Software in any applications, frameworks, or elements (collectively 
// referred to as "Applications") that you develop using the Software in accordance with this 
// License Agreement, provided that such distribution does not violate the restrictions set 
// forth in section 4 of this agreement.

// You will not owe Us any royalties for Your distribution of the Software in accordance with 
// this License Agreement.

// 4. PROHIBITED USES
// You may not redistribute the Software or Modifications other than by including the Software 
// or a portion thereof within Your own product, which must have substantially different 
// functionality than the Software or Modifications and must not allow any third party to use 
// the Software or Modifications, or any portions thereof, for software development or application 
// development purposes. You are explicitly not allowed to redistribute the Software or 
// Modifications as part of any product that can be described as a development toolkit or library 
// or is intended for use by software developers or application developers and not end-users.

// 5. TERMINATION
// This Agreement shall terminate automatically if you fail to comply with the limitations 
// described in this Agreement. No notice shall be required to effectuate such termination. 
// Upon termination, you must remove and destroy all copies of the Software. 

// 6. DISCLAIMER OF WARRANTY
// YOU AGREE THAT WE HAVE MADE NO EXPRESS WARRANTIES, ORAL OR WRITTEN, TO YOU REGARDING THE 
// SOFTWARE AND THAT THE SOFTWARE IS BEING PROVIDED TO YOU 'AS IS' WITHOUT WARRANTY OF ANY KIND.
//  WE DISCLAIM ANY AND ALL OTHER WARRANTIES, WHETHER EXPRESSED, IMPLIED, OR STATUTORY. YOUR RIGHTS
//  MAY VARY DEPENDING ON THE STATE IN WHICH YOU LIVE. WE SHALL NOT BE LIABLE FOR INDIRECT, 
// INCIDENTAL, SPECIAL, COVER, RELIANCE, OR CONSEQUENTIAL DAMAGES RESULTING FROM THE USE OF THIS PRODUCT.

// 7. LIMITATION OF LIABILITY
// YOU USE THIS PROGRAM SOLELY AT YOUR OWN RISK. IN NO EVENT SHALL WE BE LIABLE TO YOU FOR ANY DAMAGES,
// INCLUDING BUT NOT LIMITED TO ANY LOSS, OR OTHER INCIDENTAL, INDIRECT OR CONSEQUENTIAL DAMAGES OF 
// ANY KIND ARISING OUT OF THE USE OF THE SOFTWARE, EVEN IF WE HAVE BEEN ADVISED OF THE POSSIBILITY OF
// SUCH DAMAGES. IN NO EVENT WILL WE BE LIABLE FOR ANY CLAIM, WHETHER IN CONTRACT, TORT, OR ANY OTHER
// THEORY OF LIABILITY, EXCEED THE COST OF THE SOFTWARE. THIS LIMITATION SHALL APPLY TO CLAIMS OF 
// PERSONAL INJURY TO THE EXTENT PERMITTED BY LAW.

// 8. MISCELLANEOUS
// The license granted herein applies only to the version of the Software available when acquired
// in connection with the terms of this Agreement. Any previous or subsequent license granted to
// You for use of the Software shall be governed by the terms and conditions of the agreement entered
// in connection with the acquisition of that version of the Software. You agree that you will comply
// with all applicable laws and regulations with respect to the Software, including without limitation
// all export and re-export control laws and regulations.
#endregion


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Bitrave.Azure;

public class AzureUI : MonoBehaviour {

    private ToDoItem _todo = new ToDoItem()
        {
            Text = "Some Text",
            Complete = false
        };

    public List<ToDoItem> _toDoItems = new List<ToDoItem>();

    public string AzureEndPoint = "https://bitraveservices.azure-mobile.net/";
    public string ApplicationKey = "ePYhutMVmiUPWhFAJaRYTJsPFTiuAB20";
    public string FacebookAccessToken = "xxx";
    private AzureMobileServices azure;

	// Use this for initialization
	void Start () {
        azure = new AzureMobileServices(AzureEndPoint, ApplicationKey);
    }

    private void Login()
    {
        azure.LoginAsync(AuthenticationProvider.Facebook,
                        FacebookAccessToken,
                        LoginAsyncCallback);
    }


    private void LoginAsyncCallback(AzureResponse<MobileServiceUser> obj)
    {
        if (obj.Status == AzureResponseStatus.Success)
        {
            azure.User = obj.ResponseData;
        }
        else
        {
            Debug.Log("Error:" + obj.StatusCode);
        }
    }

    public void GetAllItems()
    {
        _toDoItems.Clear();
        azure.Where<ToDoItem>(p => p.Text != null, ReadHandler);
    }

    public void ReadHandler(AzureResponse<List<ToDoItem>> response)
    {

        var list = response.ResponseData;

        Debug.Log("Items ==================");
        foreach (var item in list)
        {
            Debug.Log(item.Text + "," + item.Id);
            _toDoItems.Add(item);
        }
        Debug.Log("==================");
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    private Vector2 scrollPosition;
    
    public void OnGUI()
    {
        GUILayout.BeginVertical();        
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical(GUILayout.Width(200));
        GUILayout.Label("Azure End Point");
        AzureEndPoint = GUILayout.TextField(AzureEndPoint, GUILayout.Width(200));
        GUILayout.Label("Application Key");
        ApplicationKey = GUILayout.TextField(ApplicationKey, GUILayout.Width(200));
        GUILayout.Label("Facebook Access Token");
        FacebookAccessToken = GUILayout.TextField(FacebookAccessToken, GUILayout.Width(200));
        if (GUILayout.Button("Log In")) Login();
        GUILayout.EndVertical();

        GUI.enabled = (azure != null && azure.User != null);

        GUILayout.BeginVertical(GUILayout.Width(200));
        GUILayout.TextField(""+_todo.Id);
        _todo.Text = GUILayout.TextField(_todo.Text);
        if(GUILayout.Button("Add"))
        {
            // Note: You don't need to do the following, 
            // it's done in the insert method. 
            // _todo.Id = null;
            azure.Insert<ToDoItem>(_todo);
        }
        GUILayout.EndVertical();
        GUILayout.BeginVertical(GUILayout.Width(200));

        if (GUILayout.Button("Query/Where"))
        {
            _toDoItems.Clear();
            azure.Where<ToDoItem>( p => p.Text == "Some Text", ReadHandler);
        }
        if (GUILayout.Button("List All"))
        {
            GetAllItems();
        }

        GUILayout.Label("Item count: " + _toDoItems.Count);

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.Height(300));
        
        GUILayout.BeginVertical();
        foreach (var item in _toDoItems)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(">", GUILayout.Width(20), GUILayout.Height(20)))
            {
                _selectedItem = item;
            }
            GUILayout.Label(item.Text);
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
        
        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.BeginVertical(GUILayout.Width(200));

        var was = GUI.enabled;

        GUI.enabled = _selectedItem.Id != null;

        GUILayout.Label("ID:" + _selectedItem.Id);
        _selectedItem.Text = GUILayout.TextField(_selectedItem.Text);
        _selectedItem.Complete = GUILayout.Toggle(_selectedItem.Complete, "Complete");

        if (GUILayout.Button("Update"))
        {
            azure.Update<ToDoItem>(_selectedItem);
        }
        if (GUILayout.Button("Delete"))
        {
            azure.Delete<ToDoItem>(_selectedItem);
        }

        GUI.enabled = was;

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUI.enabled = true;
    }

    private ToDoItem _selectedItem = new ToDoItem()
    {
        Complete = false,
        Text = ""
    };
}
