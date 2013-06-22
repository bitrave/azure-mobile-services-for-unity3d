azure-mobile-services-for-unity3d
=================================

The solution is broken up into a few projects, with shared source.  The following covers briefly to what they are, and how to build the project.

**Bitrave.Azure.Editor**
This is the Azure client for running inside the Unity Editor.  It currently is synchronous, and doesn't have a full feature set.  It's handy for testing against real data sets.  The plan is to make it fully featured at some point.

**RestSharp.Stub**
This project is a stub to get around building out of Unity for Windows Phone 8.  It can be deleted before deploying to the phone.

**Bitrave.Azure.Windows8**
This is the plugin for Unity based Windows 8 Store apps.

**Bitrave.Azure.WindowsPhone8**
This is the plugin for Unity based Windows Phone 8 Store apps.  It currently doesn't support Login due to a Unity build issue.  Once that's fixed, we'll have login working again.

Bitrave.Azure.Windows8.TestApp and Bitrave.Azure.WindowsPhone8.TestApp are just apps that help test various bits of functionality if you are working on the plugin.  Due to Unity's early days for Windows 8 and Windows Phone 8, this lets you check whether it's your Unity build that's failing, or whether the plugin is falling over, since you can't debug the plugin in Unity.

**Building**
Building is simple, just hit build.  Make sure you have the dependencies from Nuget.  You will want to grab the latest JSON.NET and RestSharp, in addition to the Azure Mobile Services SDK.

When you build, the files get copied across to the Output folder.  The Plugins folder in this folder needs to be copied across to your Unity/Assets folder.

