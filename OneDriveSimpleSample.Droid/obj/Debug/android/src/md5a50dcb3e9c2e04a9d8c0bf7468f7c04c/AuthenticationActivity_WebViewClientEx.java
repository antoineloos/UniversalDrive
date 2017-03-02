package md5a50dcb3e9c2e04a9d8c0bf7468f7c04c;


public class AuthenticationActivity_WebViewClientEx
	extends android.webkit.WebViewClient
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onPageFinished:(Landroid/webkit/WebView;Ljava/lang/String;)V:GetOnPageFinished_Landroid_webkit_WebView_Ljava_lang_String_Handler\n" +
			"n_onReceivedError:(Landroid/webkit/WebView;ILjava/lang/String;Ljava/lang/String;)V:GetOnReceivedError_Landroid_webkit_WebView_ILjava_lang_String_Ljava_lang_String_Handler\n" +
			"";
		mono.android.Runtime.register ("OneDriveSimpleSample.Droid.AuthenticationActivity+WebViewClientEx, OneDriveSimpleSample.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", AuthenticationActivity_WebViewClientEx.class, __md_methods);
	}


	public AuthenticationActivity_WebViewClientEx () throws java.lang.Throwable
	{
		super ();
		if (getClass () == AuthenticationActivity_WebViewClientEx.class)
			mono.android.TypeManager.Activate ("OneDriveSimpleSample.Droid.AuthenticationActivity+WebViewClientEx, OneDriveSimpleSample.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onPageFinished (android.webkit.WebView p0, java.lang.String p1)
	{
		n_onPageFinished (p0, p1);
	}

	private native void n_onPageFinished (android.webkit.WebView p0, java.lang.String p1);


	public void onReceivedError (android.webkit.WebView p0, int p1, java.lang.String p2, java.lang.String p3)
	{
		n_onReceivedError (p0, p1, p2, p3);
	}

	private native void n_onReceivedError (android.webkit.WebView p0, int p1, java.lang.String p2, java.lang.String p3);

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
