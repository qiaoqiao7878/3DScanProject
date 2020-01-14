using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;

public class ConfigMenu : MonoBehaviour
{
    //public Material[] material;
    //private SkinnedMeshRenderer renderer;
    private double bodyheight = 1.7;
    public TextAsset jointRegressorJSON;
    private string _gender = "male";
    private SMPLJointCalculator _jointCalculator;
    private int _numJoints = 24;
    private int _numShapeParms = 10;
    private float[] _shapeParms;
    private string json;

    void Awake()
    {
        _shapeParms = new float[_numShapeParms];
        resetShapeParms();

        _jointCalculator = new SMPLJointCalculator(jointRegressorJSON, _numJoints, _shapeParms.Length);
        if (!_jointCalculator.initialize())
        {
            Debug.Log("ERROR: Failed to initialize SMPLJointCalculator object");
            Application.Quit();
        }
    }

    public void resetShapeParms()
    {
        for (int bi = 0; bi < _numShapeParms; bi++)
            _shapeParms[bi] = 0.0f;
    }

    void Start() {
        _gender = _jointCalculator.getGender();
        if (_gender != "male" && _gender != "female")
        {
            Debug.LogError("WARNING: Invalid gender.. Setting gender to female");
            _gender = "female";
        }
    }

    public void scanning()
    {
       
        bodyheight = _jointCalculator.getBodyheight();

        Bodyshape_female bodyshape_female = new Bodyshape_female();
        bodyshape_female.betas[0] = (bodyheight - 1.7) * 7;
        Bodyshape_male bodyshape_male = new Bodyshape_male();
        bodyshape_male.betas[0] = (bodyheight - 1.7) * 7;

        if (_gender == "male"){
            json = JsonUtility.ToJson(bodyshape_male);
        }
        else {
            json = JsonUtility.ToJson(bodyshape_female);
        }

        File.WriteAllText("Assets/smpl/Samples/Betas/user.json", json);

        UnityEditor.AssetDatabase.Refresh();
        SceneManager.LoadScene(1);
    }
    private class Bodyshape_female
    {
        public double[] betas = new double[10] {-1.50338909, 0.41133214, -0.31445075, -0.90729174, 0.89161303, -1.1674648, -0.36843207, -0.42175958, 1.00391208, 1.2608627};
    }
    private class Bodyshape_male
    {
        public double[] betas = new double[10] { 0.55717977, -1.81291238, -0.54321285,  0.23705893, -0.50107065,1.24639222,  0.43375487,  0.15281353, -0.23500944,  0.10896058 };
    }

    public void turnClock()
    {

    }

    public void turnCounterClock()
    {

    }

    public void save()
    {

    }

    public void back()
    {
        SceneManager.LoadScene(0);
    }
}
