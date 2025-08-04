using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xb.pipe {
    public class UserRepository {

        public static UserRepository Instance = new UserRepository();

        public static Action<int> OnBestScoreChanged;


        public int BestScore {
            get {
                return PlayerPrefs.GetInt("Best", 0);
            }
            set {
                PlayerPrefs.SetInt("Best", value);
                PlayerPrefs.Save();
                OnBestScoreChanged?.Invoke(value);
            }
        }

    }
}
