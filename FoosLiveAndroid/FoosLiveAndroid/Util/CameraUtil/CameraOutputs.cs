using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Hardware.Camera2;
using Java.Lang;
using Android.Hardware.Camera2.Params;
using Java.Util;
using Android.Util;
using Android.Graphics;
using Android.Media;
using Android.Content.Res;

namespace FoosLiveAndroid.Util.CameraUtil
{
    class CameraOutputs
    {
        private static int MAX_PREVIEW_WIDTH = 1920;
        private static int MAX_PREVIEW_HEIGHT = 1080;
        public static void SetUp(Activity activity, ImageReader reader, int width, int height,
                    out Size previewSize, AutoFitTextureView view, out string CameraID, out bool flashSupported)
        {
            CameraManager manager = (CameraManager)activity.GetSystemService(Context.CameraService);
            int sensorOrientation;
            try
            {
                for (int i = 0 ; i < manager.GetCameraIdList().Length ; i ++)
                {
                    string cameraID = manager.GetCameraIdList()[i];
                    CameraCharacteristics characteristics = manager.GetCameraCharacteristics(cameraID);

                    /**
                     * Check if it's front facing
                     */
                    int facing = (int)characteristics.Get(CameraCharacteristics.LensFacing);
                    if (facing == (int)LensFacing.Front)
                    {
                        continue;
                    }

                    StreamConfigurationMap map = (StreamConfigurationMap)characteristics.Get(CameraCharacteristics.ScalerStreamConfigurationMap);
                    if (map == null)
                    {
                        continue;
                    }

                    Size largest = (Size)Collections.Max(Arrays.AsList(map.GetOutputSizes((int)ImageFormatType.Jpeg)));
                    reader = ImageReader.NewInstance(largest.Width, largest.Height, ImageFormatType.Jpeg, 2);
                    /**
                     * TODO
                     * Add listeners
                     */
                    //reader.SetOnImageAvailableListener()

                    SurfaceOrientation rotation = activity.WindowManager.DefaultDisplay.Rotation;
                    sensorOrientation = (int)characteristics.Get(CameraCharacteristics.SensorOrientation);
                    bool swappedDimensions = false;

                    switch(rotation)
                    {
                        case SurfaceOrientation.Rotation0:
                        case SurfaceOrientation.Rotation180:
                            if (sensorOrientation == 90 || sensorOrientation == 270)
                            {
                                swappedDimensions = true;
                            }
                            break;
                        case SurfaceOrientation.Rotation90:
                        case SurfaceOrientation.Rotation270:
                            if (sensorOrientation == 0 || sensorOrientation == 180)
                            {
                                swappedDimensions = true;
                            }
                            break;
                        default:
                            /**
                             * Display error message
                             */
                            break;
                    }

                    Point displaySize = new Point();
                    activity.WindowManager.DefaultDisplay.GetSize(displaySize);
                    var rotatedPreviewWidth = width;
                    var rotatedPreviewHeight = height;
                    var maxPreviewWidth = displaySize.X;
                    var maxPreviewHeight = displaySize.Y;

                    if(swappedDimensions)
                    {
                        rotatedPreviewWidth = height;
                        rotatedPreviewHeight = width;
                        maxPreviewWidth = displaySize.Y;
                        maxPreviewHeight = displaySize.X;
                    }

                    if (maxPreviewWidth > MAX_PREVIEW_WIDTH)
                    {
                        maxPreviewWidth = MAX_PREVIEW_WIDTH;
                    }

                    if (maxPreviewHeight > MAX_PREVIEW_HEIGHT)
                    {
                        maxPreviewHeight = MAX_PREVIEW_HEIGHT;
                    }

                    previewSize = CameraOptimalSize.Get(map.GetOutputSizes(Class.FromType(typeof(SurfaceTexture))),
                        rotatedPreviewWidth, rotatedPreviewHeight, maxPreviewWidth,
                        maxPreviewHeight, largest);

                    var orientation = activity.Resources.Configuration.Orientation;
                    if (orientation == Android.Content.Res.Orientation.Landscape)
                    {
                        view.SetAspectRatio(previewSize.Width, previewSize.Height);
                    }
                    else
                    {
                        view.SetAspectRatio(previewSize.Height, previewSize.Width);
                    }

                    // Check if the flash is supported.
                    var available = (bool)characteristics.Get(CameraCharacteristics.FlashInfoAvailable);
                    if (!available)
                    {
                        flashSupported = false;
                    }
                    else
                    {
                        flashSupported = true;
                    }

                    CameraID = cameraID;
                    return;
                }
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
            catch (NullPointerException e)
            {
                e.PrintStackTrace();
                // Nezinau kaip cia elgtis
            }
            previewSize = null;
            CameraID = null;
            flashSupported = false;
        }
    }
}