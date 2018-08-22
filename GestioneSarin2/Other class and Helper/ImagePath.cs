using System;
using Android.Annotation;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Provider;
using Java.Lang;
using Environment = Android.OS.Environment;
using String = System.String;
using Uri = Android.Net.Uri;

//using android.provider.<span id="IL_AD11" class="IL_AD">MediaStore</span>;

public class ImageFilePath
{

    /**
     * Method for return file path of Gallery image
     *
     * @param context
     * @param uri
     * @return path of the selected image file from gallery
     */
    static string nopath = "Select Video Only";

    public static string getPath( Context context,  Uri uri)
    {

        // check here to KITKAT or new version
         bool isKitKat = Build.VERSION.SdkInt >= Build.VERSION_CODES.Kitkat;

        // DocumentProvider
        if (isKitKat && DocumentsContract.IsDocumentUri(context, uri))
        {

            // ExternalStorageProvider
            if (isExternalStorageDocument(uri))
            {
                 string docId = DocumentsContract.GetDocumentId(uri);
                 string[] split = docId.Split(':');
                 string type = split[0];

                if ("primary".Equals(type,StringComparison.OrdinalIgnoreCase))
                {
                    return Environment.ExternalStorageDirectory + "/"
                            + split[1];
                }
            }
            // DownloadsProvider
            else if (isDownloadsDocument(uri))
            {
                 String id = DocumentsContract.GetDocumentId(uri);
                 Uri contentUri = ContentUris.WithAppendedId(
                        Uri.Parse("content://downloads/public_downloads"),
                        long.Parse(id));

                return getDataColumn(context, contentUri, null, null);
            }
            // MediaProvider
            else if (isMediaDocument(uri))
            {
                 String docId = DocumentsContract.GetDocumentId(uri);
                 String[] split = docId.Split(':');
                 String type = split[0];

                Uri contentUri = null;
                if ("image".Equals(type))
                {
                    contentUri = MediaStore.Images.Media.ExternalContentUri;
                }
                else if ("video".Equals(type))
                {
                    contentUri = MediaStore.Video.Media.ExternalContentUri;
                }
                else if ("audio".Equals(type))
                {
                    contentUri = MediaStore.Audio.Media.ExternalContentUri;
                }

                 String selection = "_id=?";
                 String[] selectionArgs = new String[] { split[1] };

                return getDataColumn(context, contentUri, selection,
                        selectionArgs);
            }
        }
        // MediaStore (and general)
        else if ("content".Equals(uri.Scheme,StringComparison.OrdinalIgnoreCase))
        {

            // Return the remote address
            if (isGooglePhotosUri(uri))
                return uri.LastPathSegment;

            return getDataColumn(context, uri, null, null);
        }
        // File
        else if ("file".Equals(uri.Scheme,StringComparison.OrdinalIgnoreCase))
        {
            return uri.Path;
        }

        return nopath;
    }

    
    public static String getDataColumn(Context context, Uri uri,
                                       String selection, String[] selectionArgs)
    {

        ICursor cursor = null;
         String column = "_data";
         String[] projection = { column };

        try
        {
            cursor = context.ContentResolver.Query(uri, projection,
                    selection, selectionArgs, null);
            if (cursor != null && cursor.MoveToFirst())
            {
                 int index = cursor.GetColumnIndexOrThrow(column);
                return cursor.GetString(index);
            }
        }
        finally
        {
            cursor?.Close();
        }
        return nopath;
    }

    /**
     * @param uri
     *            The Uri to check.
     * @return Whether the Uri authority is ExternalStorageProvider.
     */
    public static bool isExternalStorageDocument(Uri uri)
    {
        return "com.android.externalstorage.documents".Equals(uri
                .Authority);
    }

    /**
     * @param uri
     *            The Uri to check.
     * @return Whether the Uri authority is DownloadsProvider.
     */
    public static bool isDownloadsDocument(Uri uri)
    {
        return "com.android.providers.downloads.documents".Equals(uri
                .Authority);
    }

    /**
     * @param uri
     *            The Uri to check.
     * @return Whether the Uri authority is MediaProvider.
     */
    public static bool isMediaDocument(Uri uri)
    {
        return "com.android.providers.media.documents".Equals(uri
                .Authority);
    }

    /**
     * @param uri
     *            The Uri to check.
     * @return Whether the Uri authority is Google Photos.
     */
    public static bool isGooglePhotosUri(Uri uri)
    {
        return "com.google.android.apps.photos.content".Equals(uri
                .Authority);
    }
}