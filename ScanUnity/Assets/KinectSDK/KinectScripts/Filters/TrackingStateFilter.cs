using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Implementation of a Holt Double Exponential Smoothing filter. The double exponential
/// smooths the curve and predicts.  There is also noise jitter removal.
/// </summary>
public class TrackingStateFilter
{
    // The history data.
    private FilterDoubleExponentialData[] history;

    // The transform smoothing parameters for this filter.
    private KinectWrapper.NuiTransformSmoothParameters smoothParameters;

    // True when the filter parameters are initialized.
    private bool init;

    // Get the estimated covariance of position predicted forward from last measurement time by dt.
    // P = F * X * F^T
    //public double Variance(double dt, double m_p0, double m_p1, double m_p2, double m_p3, double m_q0)
    //{
    //    return m_p0 + dt * (m_p2 + m_p1) + dt * dt * m_p3 + m_q0;
    //}
    // State transition model
   
    

    private double[] F = new double[4];
    // Control-input model
    private double[] B = new double[2];

    private double dt = 0.001f;
    //public float a = 0;    // accelerate has to be changed
    public float a = 1;    // accelerate has to be changed
	
    /// Initializes a new instance of the class.
    public TrackingStateFilter()
    {
        this.init = false;
    }

    // Initialize the filter with a default set of TransformSmoothParameters.
    public void Init()
    {
        // Specify some defaults
        //this.Init(0.25f, 0.25f, 0.25f, 0.03f, 0.05f);
        this.Init(0.5f, 0.5f, 0.5f, 0.05f, 0.04f);
    }

    /// <summary>
    /// Initialize the filter with a set of manually specified TransformSmoothParameters.
    /// </summary>
    /// <param name="smoothingValue">Smoothing = [0..1], lower values is closer to the raw data and more noisy.</param>
    /// <param name="correctionValue">Correction = [0..1], higher values correct faster and feel more responsive.</param>
    /// <param name="predictionValue">Prediction = [0..n], how many frames into the future we want to predict.</param>
    /// <param name="jitterRadiusValue">JitterRadius = The deviation distance in m that defines jitter.</param>
    /// <param name="maxDeviationRadiusValue">MaxDeviation = The maximum distance in m that filtered positions are allowed to deviate from raw data.</param>
    public void Init(float smoothingValue, float correctionValue, float predictionValue, float jitterRadiusValue, float maxDeviationRadiusValue)
    {
        this.smoothParameters = new KinectWrapper.NuiTransformSmoothParameters();

        this.smoothParameters.fSmoothing = smoothingValue;                   // How much soothing will occur.  Will lag when too high
        this.smoothParameters.fCorrection = correctionValue;                 // How much to correct back from prediction.  Can make things springy
        this.smoothParameters.fPrediction = predictionValue;                 // Amount of prediction into the future to use. Can over shoot when too high
        this.smoothParameters.fJitterRadius = jitterRadiusValue;             // Size of the radius where jitter is removed. Can do too much smoothing when too high
        this.smoothParameters.fMaxDeviationRadius = maxDeviationRadiusValue; // Size of the max prediction radius Can snap back to noisy data when too high

        this.Reset();
        this.init = true;
    }

    // Initialize the filter with a set of TransformSmoothParameters.
    public void Init(KinectWrapper.NuiTransformSmoothParameters smoothingParameters)
    {
        this.smoothParameters = smoothingParameters;

        this.Reset();
        this.init = true;
    }

    // Resets the filter to default values.
    public void Reset()
    {
        this.history = new FilterDoubleExponentialData[(int)KinectWrapper.NuiSkeletonPositionIndex.Count];
    }

    // Update the filter with a new frame of data and smooth.
    public void UpdateFilter(ref KinectWrapper.NuiSkeletonData skeleton)
    {
        if (skeleton.eTrackingState != KinectWrapper.NuiSkeletonTrackingState.SkeletonTracked)
        {
            return;
        }

        if (this.init == false)
        {
            this.Init();    // initialize with default parameters                
        }

        // Check for divide by zero. Use an epsilon of a 10th of a millimeter
        smoothParameters.fJitterRadius = Math.Max(0.0001f, smoothParameters.fJitterRadius);

        int jointsCount = (int)KinectWrapper.NuiSkeletonPositionIndex.Count;
        for (int jointIndex = 0; jointIndex < jointsCount; jointIndex++)
        {
            FilterJoint(ref skeleton, jointIndex, ref smoothParameters);
        }
    }

    // Update the filter for one joint.  
    protected void FilterJoint(ref KinectWrapper.NuiSkeletonData skeleton, int jointIndex, ref KinectWrapper.NuiTransformSmoothParameters smoothingParameters)
    {
        float filteredState;
        float trend;
        float diffVal;
        //double velocity;

        double velocity;

        float rawState = (float)skeleton.eSkeletonPositionTrackingState[jointIndex];
        float prevFilteredState = history[jointIndex].FilteredState;
        float prevTrend = history[jointIndex].Trend;
        float prevRawState = history[jointIndex].RawState;

        float prevVelocity = history[jointIndex].Velocity;
        //double[] m_p = history[jointIndex].m_p;
        //double[] m_q = history[jointIndex].m_q;
        //double m_r = history[jointIndex].m_r;

        F[0] = 1;
        F[1] = dt;
        F[2] = 0;
        F[3] = 1;
        B[0] = 0.5 * dt * dt;
        B[1] = dt;

        // If joint is invalid, reset the filter
        if (rawState == 0f)
        {
            history[jointIndex].FrameCount = 0;
        }


        // Initial start values
        if (history[jointIndex].FrameCount == 0)
        {
            filteredState = rawState;
            trend = 0f;
            //velocity = 1;
            velocity = 1;

        }
        else if (this.history[jointIndex].FrameCount == 1)
        {
            filteredState = (rawState + prevRawState) * 0.5f;
            //velocity = (filteredState - prevFilteredState) / dt;
            velocity = (filteredState - prevFilteredState) / dt;
            diffVal = filteredState - prevFilteredState;
            trend = (diffVal * smoothingParameters.fCorrection) + (prevTrend * (1.0f - smoothingParameters.fCorrection));
        }
        else
        {
            //            // First apply jitter filter
            //            diffVal = rawState - prevFilteredState;
            //
            //            if (diffVal <= smoothingParameters.fJitterRadius)
            //            {
            //                filteredState = (rawState * (diffVal / smoothingParameters.fJitterRadius)) + (prevFilteredState * (1.0f - (diffVal / smoothingParameters.fJitterRadius)));
            //            }
            //            else
            //            {
            //                filteredState = rawState;
            //            }

            filteredState = rawState;

            // Kalman Filter
            // filteredState = (float)F[0] * prevFilteredState + (float)F[2] * prevFilteredState - (float)B[0] * a;
            // velocity = (float)F[1] * prevVelocity + (float)F[3] * prevVelocity - (float)B[1] * a;

            // Now the double exponential smoothing filter
            filteredState = (filteredState * (1.0f - smoothingParameters.fSmoothing)) + ((prevFilteredState + prevTrend) * smoothingParameters.fSmoothing);

            diffVal = filteredState - prevFilteredState;
            trend = (diffVal * smoothingParameters.fCorrection) + (prevTrend * (1.0f - smoothingParameters.fCorrection));
            // trend = 0f;
        }
    

        // Predict into the future to reduce latency
        float predictedState = filteredState + (trend * smoothingParameters.fPrediction);
        //float predictedState = filteredState + (float)velocity;
        
        filteredState = (float)F[0] * prevFilteredState + (float)F[2] * prevFilteredState - (float)B[0] * a;
        velocity = (float)F[1] * prevVelocity + (float)F[3] * prevVelocity - (float)B[1] * a;

        // Now the double exponential smoothing filter
        // filteredState = (filteredState * (1.0f - smoothingParameters.fSmoothing)) + ((prevFilteredState + prevTrend) * smoothingParameters.fSmoothing);

        // diffVal = filteredState - prevFilteredState;
        // trend = (diffVal * smoothingParameters.fCorrection) + (prevTrend * (1.0f - smoothingParameters.fCorrection));
        trend = 0f;

        // Check that we are not too far away from raw data
        diffVal = predictedState - rawState;

        if (diffVal > smoothingParameters.fMaxDeviationRadius)
        {
            predictedState = (predictedState * (smoothingParameters.fMaxDeviationRadius / diffVal)) + (rawState * (1.0f - (smoothingParameters.fMaxDeviationRadius / diffVal)));
        }

        // Save the data from this frame
        history[jointIndex].RawState = rawState;
        history[jointIndex].FilteredState = filteredState;
        history[jointIndex].Trend = trend;
        history[jointIndex].FrameCount++;
        //history[jointIndex].Velocity = (float)velocity;

        history[jointIndex].Velocity = (float)velocity;
        
        // Set the filtered data back into the joint
        skeleton.eSkeletonPositionTrackingState[jointIndex] = (KinectWrapper.NuiSkeletonPositionTrackingState)(predictedState + 0.5f);
    }

    private double Gaussian(double mu, double sigma, double x)
    {
        double coefficient = 1.0 / Math.Sqrt(2.0 * Math.PI * sigma);
        double exponential = Math.Exp(-1 * (x - mu) * (x - mu) / (2 * sigma));
        return coefficient * exponential;
    }

    // Historical Filter Data.  
    private struct FilterDoubleExponentialData
    {
        // Gets or sets Historical Tracking State.  
        public float RawState;

        // Gets or sets Historical Filtered Tracking State.  
        public float FilteredState;

        // Gets or sets Historical Trend.  
        public float Trend;

        // Gets or sets Historical FrameCount.  
        public uint FrameCount;

        // Gets or sets Velocity

        //public float Velocity;

        public float Velocity;

        //// Gets or sets Covariance
        //public double[] m_p;

        //// Gets or sets Minimal Covariance
        //public double[] m_q;

        //// Gets or sets Minimal innovative covariance, keeps filter from locking in to a solution.
        //public double m_r;
    }
}