namespace mudz
{
    using UnityEngine;
    using System.Collections.Generic;
    class TryGetKey{

    #region Integers
        public class Int{
            public static int from_int(Dictionary<int, int> dic, int value){
            foreach(KeyValuePair<int, int> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return 0;
            }
            public static int from_float(Dictionary<int, float> dic, float value){
            foreach(KeyValuePair<int, float> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return 0;
            }
            public static int from_double(Dictionary<int, double> dic, double value){
            foreach(KeyValuePair<int, double> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return 0;
            }
            public static int from_bool(Dictionary<int, bool> dic, bool value){
            foreach(KeyValuePair<int, bool> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return 0;
            }
            public static int from_char(Dictionary<int, char> dic, char value){
            foreach(KeyValuePair<int, char> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return 0;
            }
            public static int from_string(Dictionary<int, string> dic, string value){
            foreach(KeyValuePair<int, string> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return 0;
            }
        }  
    #endregion

    #region Floats
        public class Float{
            public static float from_int(Dictionary<float, int> dic, int value){
            foreach(KeyValuePair<float, int> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return 0;
            }
            public static float from_float(Dictionary<float, float> dic, float value){
            foreach(KeyValuePair<float, float> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return 0;
            }
            public static float from_double(Dictionary<float, double> dic, double value){
            foreach(KeyValuePair<float, double> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return 0;
            }
            public static float from_bool(Dictionary<float, bool> dic, bool value){
            foreach(KeyValuePair<float, bool> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return 0;
            }
            public static float from_char(Dictionary<float, char> dic, char value){
            foreach(KeyValuePair<float, char> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return 0;
            }
            public static float from_string(Dictionary<float, string> dic, string value){
            foreach(KeyValuePair<float, string> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return 0;
            }
        }
    #endregion
    
    #region Doubles
        public class Double{
            public static double from_int(Dictionary<double, int> dic, int value){
            foreach(KeyValuePair<double, int> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return 0;
            }
            public static double from_float(Dictionary<double, float> dic, float value){
            foreach(KeyValuePair<double, float> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return 0;
            }
            public static double from_double(Dictionary<double, double> dic, double value){
            foreach(KeyValuePair<double, double> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return 0;
            }
            public static double from_bool(Dictionary<double, bool> dic, bool value){
            foreach(KeyValuePair<double, bool> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return 0;
            }
            public static double from_char(Dictionary<double, char> dic, char value){
            foreach(KeyValuePair<double, char> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return 0;
            }
            public static double from_string(Dictionary<double, string> dic, string value){
            foreach(KeyValuePair<double, string> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return 0;
            }
        }
    #endregion

    #region Bools
        public class Bool{
            public static bool from_int(Dictionary<bool, int> dic, int value){
            foreach(KeyValuePair<bool, int> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return false;
            }
            public static bool from_float(Dictionary<bool, float> dic, float value){
            foreach(KeyValuePair<bool, float> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return false;
            }
            public static bool from_double(Dictionary<bool, double> dic, double value){
            foreach(KeyValuePair<bool, double> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return false;
            }
            public static bool from_bool(Dictionary<bool, bool> dic, bool value){
            foreach(KeyValuePair<bool, bool> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return false;
            }
            public static bool from_char(Dictionary<bool, char> dic, char value){
            foreach(KeyValuePair<bool, char> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return false;
            }
            public static bool from_string(Dictionary<bool, string> dic, string value){
            foreach(KeyValuePair<bool, string> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return false;
            }
        }
    #endregion

    #region Chars
        public class Char{
            public static char from_int(Dictionary<char, int> dic, int value){
            foreach(KeyValuePair<char, int> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return '#';
            }
            public static char from_float(Dictionary<char, float> dic, float value){
            foreach(KeyValuePair<char, float> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return '#';
            }
            public static char from_double(Dictionary<char, double> dic, double value){
            foreach(KeyValuePair<char, double> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return '#';
            }
            public static char from_bool(Dictionary<char, bool> dic, bool value){
            foreach(KeyValuePair<char, bool> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return '#';
            }
            public static char from_char(Dictionary<char, char> dic, char value){
            foreach(KeyValuePair<char, char> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return '#';
            }
            public static char from_string(Dictionary<char, string> dic, string value){
            foreach(KeyValuePair<char, string> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return '#';
            }
        }
    #endregion

    #region Strings
        public class String{
            public static string from_int(Dictionary<string, int> dic, int value){
            foreach(KeyValuePair<string, int> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return "#";
            }
            public static string from_float(Dictionary<string, float> dic, float value){
            foreach(KeyValuePair<string, float> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return "#";
            }
            public static string from_double(Dictionary<string, double> dic, double value){
            foreach(KeyValuePair<string, double> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return "#";
            }
            public static string from_bool(Dictionary<string, bool> dic, bool value){
            foreach(KeyValuePair<string, bool> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return "#";
            }
            public static string from_char(Dictionary<string, char> dic, char value){
            foreach(KeyValuePair<string, char> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return "#";
            }
            public static string from_string(Dictionary<string, string> dic, string value){
            foreach(KeyValuePair<string, string> pair in dic){
                    if(pair.Value == value){
                        return pair.Key;
                    }
                } return "#";
            }
        }
    #endregion
    }
    class Utils{
        public static Vector3 GetVectorFromAngle(float angle){
            // angle = 0 -> 360
            float angleRad = angle * (Mathf.PI/180f);
            return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }
        public static float GetAngleFromVectorFloat(Vector3 dir){
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if(n < 0) n += 360;

            return n;
        }
    }
}
