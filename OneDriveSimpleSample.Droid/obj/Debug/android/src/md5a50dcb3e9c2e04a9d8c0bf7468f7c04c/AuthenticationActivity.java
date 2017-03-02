package md5a50dcb3e9c2e04a9d8c0bf7468f7c04c;


public class AuthenticationActivity
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"n_onResume:()V:GetOnResumeHandler\n" +
			"";
		mono.android.Runtime.register ("OneDriveSimpleSample.Droid.AuthenticationActivity, OneDriveSimpleSample.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", AuthenticationActivity.class, __md_methods);
	}


	public AuthenticationActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == AuthenticationActivity.class)
			mono.android.TypeManager.Activate ("OneDriveSimpleSample.Droid.AuthenticationActivity, OneDriveSimpleSample.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);


	public void onResume ()
	{
		n_onResume ();
	}

	private native void n_onResume ();

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
