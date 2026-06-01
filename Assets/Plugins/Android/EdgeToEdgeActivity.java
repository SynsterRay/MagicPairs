package com.Mateusz_Bajak.MagicPairs;

import android.os.Bundle;
import androidx.activity.EdgeToEdge;
import androidx.core.view.WindowCompat;
import com.unity3d.player.UnityPlayerGameActivity;

public class EdgeToEdgeActivity extends UnityPlayerGameActivity {
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        EdgeToEdge.enable(this);
        super.onCreate(savedInstanceState);
        WindowCompat.setDecorFitsSystemWindows(getWindow(), false);
    }
}
