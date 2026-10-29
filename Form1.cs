using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Media;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;
using static System.Windows.Forms.DataFormats;
using System.Text;

namespace Subzero
{
    public partial class Form1 : Form
    {
        //Scorpion's structure
        enum ScorpionState { Idle, Throwing, Pulling, Moving, Jumping, Ducking, Sprinting, Falling, Gettingup, victory, teleport }
        ScorpionState sc_currentState = ScorpionState.Idle;

        System.Windows.Forms.Timer animationTimer = new System.Windows.Forms.Timer();
        int frameIndex = 0;

        List<Image> sc_victoryFrames, sc_idleFrames, sc_throwFrames, sc_pullFrames, sc_moveFrames, sc_jumpFrames, sc_diagJumpFrames, sc_stanceFrames, sc_duckFrames, sc_sprintFrames, sc_DstanceFrames, sc_fallFrames, sc_getUpFrames, sc_tpFrames;
        List<string> desertMusicPaths = new List<string>();

        SoundPlayer sc_sprintSound, ropeSound, getOverHereSound, comeHereSound, tpGruntSound, tpWhipSound;

        WindowsMediaPlayer bgmPlayer = new WindowsMediaPlayer();
        Point scorpionStartPos = new Point(500, 0); // X = 100, Y will be aligned to floor

        PictureBox arenaBox = new PictureBox();
        PictureBox scorpionBox = new PictureBox();
        PictureBox ropeBox = new PictureBox();
        //end of scorpion's structure

        //scorpion's booleans
        int ropeX;
        bool issc_MovingLeft = false;
        bool issc_MovingRight = false;
        bool issc_Flipped = false;
        bool isSpacePressed = false;
        bool isAKeyPressed = false;
        bool isDKeyPressed = false;
        bool isRopeFlipped = false;
        bool sc_lastFlipState = false;
        bool issc_SprintActive = false;
        bool issc_DuckPressed = false;
        bool isScorpionFrozen = false;
        bool isScDuckFlipping = false;
        bool sc_isKnockedOut = false;

        int sc_currentTrackIndex = 0;

        Random rand = new Random();
        //end of scorpion's bool

        //subzero structure
        //to merge
        enum SubzeroState { Idle, Freeze, Moving, Jumping, Ducking, Sprinting, Sliding, Gettingup, Falling, victory }
        SubzeroState sz_currentState = SubzeroState.Idle;
        int sz_frameIndex = 0;

        List<Image> sz_victoryFrames, sz_idleFrames, sz_moveFrames, sz_jumpFrames, sz_diagJumpFrames, sz_duckFrames, sz_stanceFrames, sz_DstanceFrames, sz_sprintFrames, sz_fallFrames, sz_getUpFrames;
        List<Image> sz_freezeFrames, sz_hitFrames, sz_pushFrames, sz_slideFrames;
        SoundPlayer sz_sprintSound, sz_slideSound, sz_freezeStartSound, sz_iceLaunchSound;
        PictureBox SubzeroBox = new PictureBox();
        PictureBox iceBox = new PictureBox();
        //end

        //subzero initialization
        bool issz_MovingLeft = false;
        bool issz_MovingRight = false;
        bool issz_Flipped = true;
        bool isupPressed = false;
        bool isrightPressed = false;
        bool isleftPressed = false;
        bool issz_DuckPressed = false;
        bool issz_SprintActive = false;
        bool isSzDuckFlipping = false;
        bool sz_isKnockedOut = false;
        int sz_currentTrackIndex = 0;
        int sz_globalStartX = 0;
        int sz_floorY = 0;

        //Round start stuff
        PictureBox roundBox = new PictureBox();
        PictureBox fightBox = new PictureBox();
        int currentRound = 1;
        int scorpionWins = 0;
        int subzeroWins = 0;
        bool isRoundActive = false;

        SoundPlayer round1Sound;
        SoundPlayer round2Sound;
        SoundPlayer round3Sound;
        SoundPlayer fightSound;
        //end of round start

        //healthbar stuff
        Panel healthLPanel = new Panel();
        Panel healthRPanel = new Panel();

        Label nameLabelL = new Label();
        Label nameLabelR = new Label();

        int maxHealth = 200;
        int currentHealthL = 200;
        int currentHealthR = 200;

        int redHealthL = 200;
        int redHealthR = 200;
        //end of hb

        //victory structure
        PictureBox victorySpriteBox = new PictureBox();
        PictureBox flawlessBox = new PictureBox();

        SoundPlayer scorpionWinSound;
        SoundPlayer subzeroWinSound;
        SoundPlayer flawlessSound;
        //end of victory structure

        //initialization
        public Form1()
        {

            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;
            this.KeyUp += MainForm_KeyUp;
            this.KeyDown += sz_MainForm_KeyDown;//to merge
            this.KeyUp += sz_MainForm_KeyUp;//to merge
            this.DoubleBuffered = true;
            this.Load += Form1_Load;
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Bounds = Screen.PrimaryScreen.Bounds;
        }
        private async void Form1_Load(object sender, EventArgs e)
        {
            LoadResources();
            SetupControls();
            SetupAnimationTimer();
            LoadDesertMusic();
            sz_floorY = this.ClientSize.Height - scorpionBox.Height;//to merge
            SubzeroBox.Top = sz_floorY;//to merge
            await StartRoundSequence();
        }
        //end of initialization

        //Assets shared
        private void LoadResources()
        {
            //Scorpion's Assets
            string baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Characters", "Scorpion");

            sc_idleFrames = LoadFrames(Path.Combine(baseDir, "Sprites", "Idle"));
            sc_throwFrames = LoadFrames(Path.Combine(baseDir, "Sprites", "Throw"));
            sc_pullFrames = LoadFrames(Path.Combine(baseDir, "Sprites", "Pull"));
            sc_moveFrames = LoadFrames(Path.Combine(baseDir, "Sprites", "Move"));
            sc_jumpFrames = LoadFrames(Path.Combine(baseDir, "Sprites", "Jump"));
            sc_diagJumpFrames = LoadFrames(Path.Combine(baseDir, "Sprites", "DiagJump"));
            sc_duckFrames = LoadFrames(Path.Combine(baseDir, "Sprites", "duck"));
            sc_stanceFrames = LoadFrames(Path.Combine(baseDir, "Sprites", "Stance", "stand"));
            sc_DstanceFrames = LoadFrames(Path.Combine(baseDir, "Sprites", "Stance", "duck"));
            sc_sprintFrames = LoadFrames(Path.Combine(baseDir, "Sprites", "Sprint"));
            sc_tpFrames = LoadFrames(Path.Combine(baseDir, "Sprites", "teleport"));
            sc_victoryFrames = LoadFrames(Path.Combine(baseDir, "Sprites", "victory"));
            sc_fallFrames = LoadFrames(Path.Combine(baseDir, "Sprites", "fall_getup", "fall"));
            sc_getUpFrames = LoadFrames(Path.Combine(baseDir, "Sprites", "fall_getup", "get_up"));
            ropeSound = new SoundPlayer(Path.Combine(baseDir, "Sounds", "rope.wav"));
            getOverHereSound = new SoundPlayer(Path.Combine(baseDir, "Sounds", "get_over_here.wav"));
            comeHereSound = new SoundPlayer(Path.Combine(baseDir, "Sounds", "come_here.wav"));
            sc_sprintSound = new SoundPlayer(Path.Combine(baseDir, "Sounds", "sc_sprint.wav"));
            tpGruntSound = new SoundPlayer(Path.Combine(baseDir, "Sounds", "teleport_grunt.wav"));
            tpWhipSound = new SoundPlayer(Path.Combine(baseDir, "Sounds", "teleport_whip.wav"));

            //end of scorpion's assets

            //subzero assets - to merge
            string sz_baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Characters", "Subzero");
            sz_idleFrames = LoadFrames(Path.Combine(sz_baseDir, "Sprites", "Idle"));
            sz_moveFrames = LoadFrames(Path.Combine(sz_baseDir, "Sprites", "Move"));
            sz_jumpFrames = LoadFrames(Path.Combine(sz_baseDir, "Sprites", "Jump"));
            sz_diagJumpFrames = LoadFrames(Path.Combine(sz_baseDir, "Sprites", "DiagJump"));
            sz_duckFrames = LoadFrames(Path.Combine(sz_baseDir, "Sprites", "duck"));
            sz_stanceFrames = LoadFrames(Path.Combine(sz_baseDir, "Sprites", "Stance", "stand"));
            sz_DstanceFrames = LoadFrames(Path.Combine(sz_baseDir, "Sprites", "Stance", "duck"));
            sz_sprintFrames = LoadFrames(Path.Combine(sz_baseDir, "Sprites", "Sprint"));
            sz_freezeFrames = LoadFrames(Path.Combine(sz_baseDir, "Sprites", "Freeze"));
            sz_pushFrames = LoadFrames(Path.Combine(sz_baseDir, "Sprites", "Freeze", "Ice", "push"));
            sz_hitFrames = LoadFrames(Path.Combine(sz_baseDir, "Sprites", "Freeze", "Ice", "hit"));
            sz_slideFrames = LoadFrames(Path.Combine(sz_baseDir, "Sprites", "slide"));
            sz_victoryFrames = LoadFrames(Path.Combine(sz_baseDir, "Sprites", "victory"));
            sz_fallFrames = LoadFrames(Path.Combine(sz_baseDir, "Sprites", "fall_getup", "fall"));
            sz_getUpFrames = LoadFrames(Path.Combine(sz_baseDir, "Sprites", "fall_getup", "get_up"));
            string soundDir = Path.Combine(sz_baseDir, "Sounds");
            sz_sprintSound = new SoundPlayer(Path.Combine(soundDir, "sz_sprint.wav"));
            sz_slideSound = new SoundPlayer(Path.Combine(soundDir, "slide.wav"));
            sz_freezeStartSound = new SoundPlayer(Path.Combine(soundDir, "freeze_start.wav"));
            sz_iceLaunchSound = new SoundPlayer(Path.Combine(soundDir, "ice_launch.wav"));
            //end of assets

            //Round UI resources

            // ROUND 1 banner
            roundBox.Size = new Size(350, 100);
            roundBox.SizeMode = PictureBoxSizeMode.Zoom;
            roundBox.BackColor = Color.Transparent;
            roundBox.Visible = false;
            roundBox.Parent = arenaBox;
            roundBox.BringToFront();

            // FIGHT banner
            fightBox.Size = new Size(350, 100);
            fightBox.SizeMode = PictureBoxSizeMode.Zoom;
            fightBox.BackColor = Color.Transparent;
            fightBox.Visible = false;
            fightBox.Parent = arenaBox;
            fightBox.BringToFront();

            // Load round/fight audio
            round1Sound = new SoundPlayer(Path.Combine("Assets", "Images", "UI", "round1.wav"));
            round2Sound = new SoundPlayer(Path.Combine("Assets", "Images", "UI", "round2.wav"));
            round3Sound = new SoundPlayer(Path.Combine("Assets", "Images", "UI", "round3.wav"));
            fightSound = new SoundPlayer(Path.Combine("Assets", "Images", "UI", "fight.wav"));
            //end of Round UI

            // Sprites
            victorySpriteBox.Size = new Size(300, 100);
            victorySpriteBox.SizeMode = PictureBoxSizeMode.Zoom;
            victorySpriteBox.BackColor = Color.Transparent;
            victorySpriteBox.Visible = false;
            victorySpriteBox.Parent = arenaBox;
            victorySpriteBox.BringToFront();

            flawlessBox.Size = new Size(350, 100);
            flawlessBox.SizeMode = PictureBoxSizeMode.Zoom;
            flawlessBox.BackColor = Color.Transparent;
            flawlessBox.Visible = false;
            flawlessBox.Parent = arenaBox;
            flawlessBox.BringToFront();

            // Load images
            victorySpriteBox.Image = null; // Will assign dynamically
            flawlessBox.Image = Image.FromFile(Path.Combine("Assets", "Images", "UI", "flawless.png"));

            // Load sounds
            scorpionWinSound = new SoundPlayer(Path.Combine("Assets", "Images", "UI", "scorpion_wins.wav"));
            subzeroWinSound = new SoundPlayer(Path.Combine("Assets", "Images", "UI", "subzero_wins.wav"));
            flawlessSound = new SoundPlayer(Path.Combine("Assets", "Images", "UI", "flawless.wav"));

        }
        //end of assets

        //background audio
        private void PlayNextDesertTrack()
        {
            if (desertMusicPaths.Count == 0) return;

            string path = desertMusicPaths[0]; // Only the first (and only) track
            bgmPlayer.URL = path;
            bgmPlayer.settings.setMode("loop", true); // Enable looping
            bgmPlayer.controls.play();
        }
        private void LoadDesertMusic()
        {
            string musicDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Images", "desert");
            desertMusicPaths = Directory.GetFiles(musicDir, "*.wav").OrderBy(f => f).ToList();
        }
        //end of background audio
        private List<Image> LoadFrames(string folder)
        {
            List<Image> images = new List<Image>();
            string[] files = Directory.GetFiles(folder, "*.png").OrderBy(f => f).ToArray();
            foreach (string file in files)
            {
                images.Add(Image.FromFile(file));
            }
            return images;
        }
        //UI
        private void SetupControls()
        {
            arenaBox.Dock = DockStyle.Fill;
            arenaBox.SizeMode = PictureBoxSizeMode.StretchImage;
            arenaBox.Image = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Images", "subway", "arena.png"));
            this.Controls.Add(arenaBox);

            //scorpion stuff
            scorpionBox.Size = new Size(400, 400);
            scorpionBox.Location = new Point(500, this.ClientSize.Height - 401);
            scorpionBox.SizeMode = PictureBoxSizeMode.Zoom;
            scorpionBox.BackColor = Color.Transparent;
            scorpionBox.Parent = arenaBox;
            scorpionBox.BringToFront();

            //subzero stuff
            SubzeroBox.Size = new Size(400, 400);
            sz_globalStartX = this.ClientSize.Width - 900;
            SubzeroBox.Location = new Point(sz_globalStartX, sz_floorY);
            SubzeroBox.SizeMode = PictureBoxSizeMode.Zoom;
            SubzeroBox.BackColor = Color.Transparent;
            SubzeroBox.Parent = arenaBox;
            SubzeroBox.BringToFront();

            ropeBox.Size = new Size(900, 80);
            ropeBox.SizeMode = PictureBoxSizeMode.Zoom;
            ropeBox.Image = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Characters", "Scorpion", "Sprites", "rope.png"));
            ropeBox.BackColor = Color.Transparent;
            ropeBox.Visible = false;
            ropeBox.Parent = arenaBox;
            ropeBox.BringToFront();

            iceBox.Size = new Size(400, 200); // Enlarged shards
            iceBox.SizeMode = PictureBoxSizeMode.Zoom;
            iceBox.BackColor = Color.Transparent;
            iceBox.Visible = false;
            iceBox.Parent = arenaBox;
            iceBox.BringToFront();
            //end of stuff
            // === HEALTH BARS ===
            int barWidth = 850;
            int barHeight = 50;

            // Left Health Bar
            healthLPanel.Size = new Size(barWidth, barHeight);
            healthLPanel.Location = new Point(10, 40);
            healthLPanel.BackColor = Color.Transparent;
            healthLPanel.Parent = arenaBox;
            healthLPanel.BringToFront();
            healthLPanel.Paint += (s, e) => DrawHealthBar(e.Graphics, currentHealthL, redHealthL, false);

            // Right Health Bar
            healthRPanel.Size = new Size(barWidth, barHeight);
            healthRPanel.Location = new Point(this.ClientSize.Width - barWidth - 10, 40);
            healthRPanel.BackColor = Color.Transparent;
            healthRPanel.Parent = arenaBox;
            healthRPanel.BringToFront();
            healthRPanel.Paint += (s, e) => DrawHealthBar(e.Graphics, currentHealthR, redHealthR, true);

            // === NAME LABELS ===
            nameLabelL.Text = "SCORPION";
            nameLabelL.Font = new Font("Arial", 20, FontStyle.Bold);
            nameLabelL.ForeColor = Color.White;
            nameLabelL.BackColor = Color.Transparent;
            nameLabelL.AutoSize = true;
            nameLabelL.Parent = arenaBox;
            nameLabelL.Location = new Point(10, 2); // above left bar
            nameLabelL.BringToFront();

            nameLabelR.Text = "SUB-ZERO";
            nameLabelR.Font = new Font("Arial", 20, FontStyle.Bold);
            nameLabelR.ForeColor = Color.White;
            nameLabelR.BackColor = Color.Transparent;
            nameLabelR.AutoSize = true;
            nameLabelR.Parent = arenaBox;
            nameLabelR.Location = new Point(this.ClientSize.Width - 220, 2); // above right bar
            nameLabelR.BringToFront();
        }
        //end of UI

        //animation related
        private void SetupAnimationTimer()
        {
            animationTimer.Interval = 1000 / 15;
            animationTimer.Tick += (s, e) =>
            {
                UpdateCharacterFacing();
            };
            animationTimer.Tick += sc_AnimationTick;
            animationTimer.Tick += sz_AnimationTick; //to merge
            animationTimer.Start();
        }
        private void sc_AnimationTick(object sender, EventArgs e)
        {
            if (sc_currentState != ScorpionState.Idle)
                return;

            Image idleFrame = sc_idleFrames[frameIndex];
            if (issc_Flipped)
            {
                scorpionBox.Image = FlipImage(idleFrame);
            }
            else
            {
                scorpionBox.Image = idleFrame;
            }

            frameIndex = frameIndex + 1;
            if (frameIndex >= sc_idleFrames.Count)
            {
                frameIndex = 0;
            }
        }
        private void sz_AnimationTick(object sender, EventArgs e) //to merge
        {
            if (sz_currentState != SubzeroState.Idle) return;

            Image idleFrame = sz_idleFrames[sz_frameIndex];
            SubzeroBox.Image = issz_Flipped ? FlipImage(idleFrame) : idleFrame;

            sz_frameIndex++;
            if (sz_frameIndex >= sz_idleFrames.Count)
            {
                sz_frameIndex = 0;
            }
        }
        //stance update
        private void UpdateCharacterFacing()
        {
            if (!isRoundActive) return;

            // Scorpion faces Sub-Zero
            bool shouldFlipSc = scorpionBox.Left > SubzeroBox.Left;

            if (shouldFlipSc != issc_Flipped)
            {
                if (sc_currentState == ScorpionState.Ducking)
                {
                    _ = sc_PlayDuckStanceFlip(); // duck stance flip
                }
                else
                {
                    issc_Flipped = shouldFlipSc;
                    scorpionBox.Image = issc_Flipped ? FlipImage(scorpionBox.Image) : scorpionBox.Image;
                }
            }

            // Sub-Zero faces Scorpion
            bool shouldFlipSz = SubzeroBox.Left > scorpionBox.Left;

            if (shouldFlipSz != issz_Flipped)
            {
                if (sz_currentState == SubzeroState.Ducking)
                {
                    _ = sz_PlayDuckStanceFlip(); // duck stance flip
                }
                else
                {
                    issz_Flipped = shouldFlipSz;
                    SubzeroBox.Image = issz_Flipped ? FlipImage(SubzeroBox.Image) : SubzeroBox.Image;
                }
            }
            if (sc_isKnockedOut) return;
            if (sz_isKnockedOut) return;

        }
        //end of animation related

        //round start
        private async Task StartRoundSequence()
        {
            UpdateCharacterFacing();

            isRoundActive = false;

            string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Images");

            // Display "Round X"
            roundBox.Image = Image.FromFile(Path.Combine(basePath, "UI", $"round{currentRound}.png"));
            roundBox.Location = new Point(this.ClientSize.Width / 2 - 175, this.ClientSize.Height / 2 - 150);
            roundBox.Visible = true;

            try
            {
                if (currentRound == 1) round1Sound.Play();
                else if (currentRound == 2) round2Sound.Play();
                else if (currentRound == 3) round3Sound.Play();
            }
            catch { }

            await Task.Delay(2000);
            roundBox.Visible = false;

            // FIGHT flicker
            Image fightYellow = Image.FromFile(Path.Combine(basePath, "UI", "fight_yellow.png"));
            Image fightRed = Image.FromFile(Path.Combine(basePath, "UI", "fight_red.png"));

            fightBox.Location = new Point(this.ClientSize.Width / 2 - 175, this.ClientSize.Height / 2 - 50);
            fightBox.Visible = true;

            try { fightSound.Play(); } catch { }

            for (int i = 0; i < 6; i++)
            {
                fightBox.Image = (i % 2 == 0) ? fightYellow : fightRed;
                await Task.Delay(150);
            }

            fightBox.Visible = false;

            isRoundActive = true;
            PlayNextDesertTrack();
        }

        //healthbar & damage
        private void DrawHealthBar(Graphics g, int currentHealth, int redHealth, bool isRightSide)
        {
            int width = 900;
            int height = 100;

            // Border
            using (Pen borderPen = new Pen(Color.DarkRed, 100))
            {
                g.DrawRectangle(borderPen, 0, 0, width - 1, height - 1);
            }

            // Calculate bar widths
            int greenWidth = (int)((currentHealth / (float)maxHealth) * (width - 2));
            int redWidth = (int)((redHealth / (float)maxHealth) * (width - 2));

            // Red behind bar
            using (Brush redBrush = new SolidBrush(Color.Red))
            {
                if (isRightSide)
                {
                    g.FillRectangle(redBrush, width - redWidth - 1, 0, redWidth, height);
                }
                else
                {
                    g.FillRectangle(redBrush, 0, 0, redWidth, height);
                }
            }

            // Green front bar
            using (Brush greenBrush = new SolidBrush(Color.LimeGreen))
            {
                if (isRightSide)
                {
                    g.FillRectangle(greenBrush, width - greenWidth - 1, 1, greenWidth, height - 2);
                }
                else
                {
                    g.FillRectangle(greenBrush, 1, 1, greenWidth, height - 2);
                }
            }
        }
        private async void DamagePlayer(bool isLeftPlayer, int damage)
        {
            if (isLeftPlayer)
            {
                currentHealthL = Math.Max(0, currentHealthL - damage);
                healthLPanel.Invalidate();

                await Task.Delay(40);
                while (redHealthL > currentHealthL)
                {
                    redHealthL--;
                    healthLPanel.Invalidate();
                    await Task.Delay(15);
                }
            }
            else
            {
                currentHealthR = Math.Max(0, currentHealthR - damage);
                healthRPanel.Invalidate();

                await Task.Delay(40);
                while (redHealthR > currentHealthR)
                {
                    redHealthR--;
                    healthRPanel.Invalidate();
                    await Task.Delay(15);
                }

                //for damaging
                /*DamagePlayer(true, 20);  // Damage SCORPION
                DamagePlayer(false, 15); // Damage SUB-ZERO*/

            }
            //end of healthbar and damage
        }
        //win conditions
        private async void ShowVictory(string winner, bool flawless)
        {
            if (winner == "Scorpion")
            {
                await sc_victorypose();
            }
            else
            {
                await sz_victorypose();
            }

            isRoundActive = false;

            victorySpriteBox.Image = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Images", "UI", $"{winner.ToLower()}_wins.png"));
            victorySpriteBox.Location = new Point(this.ClientSize.Width / 2 - 150, this.ClientSize.Height / 2 - 150);
            victorySpriteBox.Visible = true;

            // Play name sound
            try
            {
                if (winner == "Scorpion") scorpionWinSound.Play();
                else subzeroWinSound.Play();
            }
            catch { }

            await Task.Delay(3000);

            if (flawless)
            {
                flawlessBox.Location = new Point(this.ClientSize.Width / 2 - 175, this.ClientSize.Height / 2 - 20);
                flawlessBox.Visible = true;
                try { flawlessSound.Play(); } catch { }
            }
            await Task.Delay(3000); // 2 seconds after final sound
            // === Decide match progression ===
            if (scorpionWins == 2 || subzeroWins == 2)
            {
                // Final win — do not reset
                // You may show “Game Over” banner here
            }
            else if (scorpionWins == 1 && subzeroWins == 1)
            {
                // Tie → go to Round 3
                currentRound = 3;
                await ResetMatch();
            }
            else if (scorpionWins == 1 || subzeroWins == 1)
            {
                // One round won → go to Round 2
                currentRound = 2;
                await ResetMatch();
            }
        }
        private async void CheckForKO()
        {

            if (currentHealthL == 0)
            {
                sc_isKnockedOut = true;
                bgmPlayer.controls.stop();
                subzeroWins++;
                bool flawless = currentHealthR == maxHealth;
                ShowVictory("SubZero", flawless);
            }
            else if (currentHealthR == 0)
            {
                sz_isKnockedOut = true;
                bgmPlayer.controls.stop();
                scorpionWins++;
                bool flawless = currentHealthL == maxHealth;
                ShowVictory("Scorpion", flawless);
            }
        }

        private async Task ResetMatch()
        {
            // Reset health
            currentHealthL = maxHealth;
            redHealthL = maxHealth;
            currentHealthR = maxHealth;
            redHealthR = maxHealth;

            // Hide banners
            victorySpriteBox.Visible = false;
            flawlessBox.Visible = false;

            // === Reset Scorpion's position ===
            scorpionBox.Left = scorpionStartPos.X;
            scorpionBox.Top = this.ClientSize.Height - scorpionBox.Height; // Align to floor

            SubzeroBox.Left = sz_globalStartX;
            SubzeroBox.Top = this.ClientSize.Height - SubzeroBox.Height;

            // Reset stance/direction if needed
            issc_Flipped = false;
            isRopeFlipped = false;
            issz_Flipped = true;

            // Refresh health bars
            healthLPanel.Invalidate();
            healthRPanel.Invalidate();

            //refresh players
            isScorpionFrozen = false;
            sc_isKnockedOut = false;
            sz_isKnockedOut = false;

            sc_currentState = ScorpionState.Idle;
            sz_currentState = SubzeroState.Idle;
            SetScorpionSize(400, 400);
            SetSubzeroSize(400, 400);
            frameIndex = 0;
            sz_frameIndex = 0;
            UpdateCharacterFacing();

            await StartRoundSequence(); // <-- added here to trigger next round intro

        }
        //end of win conditions

        //Scorpion's Functionalities
        private void SetScorpionSize(int width, int height)
        {
            int bottom = scorpionBox.Top + scorpionBox.Height;
            scorpionBox.Size = new Size(width, height);
            scorpionBox.Top = bottom - scorpionBox.Height;

            // Clamp to floor
            int floorY = this.ClientSize.Height - scorpionBox.Height;
            if (scorpionBox.Top > floorY)
            {
                scorpionBox.Top = floorY;
            }
        }
        private async void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Application.Exit();
            }

            if (!isRoundActive) return;

            if (e.KeyCode == Keys.Space) { isSpacePressed = true; }
            if (e.KeyCode == Keys.A) { isAKeyPressed = true; }
            if (e.KeyCode == Keys.D) { isDKeyPressed = true; }

            if (e.KeyCode == Keys.S)
            {
                issc_DuckPressed = true;
                
                if (sc_currentState == ScorpionState.Idle || sc_currentState == ScorpionState.Moving)
                {
                    _ = sc_StartDuckSequence(); // If not ducking already
                }
            }

            if (e.KeyCode == Keys.ShiftKey)
            {
                issc_SprintActive = !issc_SprintActive;
            }

            if (e.KeyCode == Keys.G && sc_currentState == ScorpionState.Idle)
            {
                _ = sc_StartThrowSequence();
            }

            //  Swap front/backflip depending on direction
            if (sc_currentState != ScorpionState.Jumping && sc_currentState != ScorpionState.Ducking)
            {
                if (e.KeyCode == Keys.E)
                {
                    if (issc_Flipped == false)
                    {
                        _ = sc_StartFrontFlip();
                    }
                    else
                    {
                        _ = sc_StartBackFlip();
                    }
                }

                if (e.KeyCode == Keys.Q)
                {
                    if (issc_Flipped == false)
                    {
                        _ = sc_StartBackFlip();
                    }
                    else
                    {
                        _ = sc_StartFrontFlip();
                    }
                }
            }

            if ((e.KeyCode == Keys.A || e.KeyCode == Keys.D) &&
                sc_currentState == ScorpionState.Idle &&
                (isAKeyPressed == true || isDKeyPressed == true))
            {
                _ = sc_StartMoveSequence();
            }

            if (e.KeyCode == Keys.Space &&
                sc_currentState != ScorpionState.Jumping &&
                sc_currentState != ScorpionState.Ducking)
            {
                _ = sc_StartJumpSequence();
            }

            if (e.KeyCode == Keys.T && sc_currentState == ScorpionState.Idle)
            {
                _ = sc_StartTeleportSequence();
            }

            if (e.KeyCode == Keys.Z)
            {
                DamagePlayer(true, 190); // SCORPION takes damage
            }
            if (e.KeyCode == Keys.X)
            {
                DamagePlayer(false, 190); // SUB-ZERO takes damage
            }
        }
        private async void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
            {
                issc_MovingLeft = false;
                isAKeyPressed = false;
            }

            if (e.KeyCode == Keys.D)
            {
                issc_MovingRight = false;
                isDKeyPressed = false;
            }

            if (e.KeyCode == Keys.S)
            {
                issc_DuckPressed = false;

                if (sc_currentState == ScorpionState.Ducking)
                {
                    _ = sc_PlayStandFromDuck();
                }
            }

            if (e.KeyCode == Keys.Space)
            {
                isSpacePressed = false;
            }
        }
        private void ToggleSprint()
        {
            bool sc_canSprint = false;
            if (issc_Flipped == false && isDKeyPressed == true)
            {
                sc_canSprint = true;
            }
            if (issc_Flipped == true && isAKeyPressed == true)
            {
                sc_canSprint = true;
            }

            if (sc_canSprint == true)
            {
                issc_SprintActive = !issc_SprintActive;
            }
        }
        private async Task sc_StartDuckSequence()
        {
            if (!isRoundActive) return;

            sc_currentState = ScorpionState.Ducking;

            int bottom = scorpionBox.Top - scorpionBox.Height;
            SetScorpionSize(220, 220);

            for (int i = 0; i < sc_duckFrames.Count; i++)
            {
                Image frame = sc_duckFrames[i];

                if (issc_Flipped == true)
                {
                    scorpionBox.Image = FlipImage(frame);
                }
                else
                {
                    scorpionBox.Image = frame;
                }

                await Task.Delay(70);
                UpdateCharacterFacing();

            }

            // Freeze last frame
            Image lastFrame = sc_duckFrames[sc_duckFrames.Count - 1];
            if (issc_Flipped == true)
            {
                scorpionBox.Image = FlipImage(lastFrame);

            }
            else
            {
                scorpionBox.Image = lastFrame;
            }
        }
        private async Task sc_PlayStandFromDuck()
        {
            if (!isRoundActive) return;

            sc_currentState = ScorpionState.Moving;

            for (int i = sc_duckFrames.Count - 1; i >= 0; i--)
            {
                Image frame = sc_duckFrames[i];

                if (issc_Flipped == true)
                {
                    scorpionBox.Image = FlipImage(frame);
                }
                else
                {
                    scorpionBox.Image = frame;
                }

                await Task.Delay(70);

            }
            SetScorpionSize(400, 400); // Return to normal size
            sc_currentState = ScorpionState.Idle;
            frameIndex = 0;
            UpdateCharacterFacing();
        }
        private async Task sc_PlayDuckStanceFlip()
        {
            if (isScDuckFlipping) return;

            isScDuckFlipping = true;
            sc_currentState = ScorpionState.Ducking;

            // Play full duck stance flip animation
            for (int i = 0; i < sc_DstanceFrames.Count; i++)
            {
                scorpionBox.Image = issc_Flipped ? FlipImage(sc_DstanceFrames[i]) : sc_DstanceFrames[i];
                await Task.Delay(70);
            }

            // Flip direction
            issc_Flipped = !issc_Flipped;

            // After flip, set image to final duck frame (holding position)
            Image finalDuckFrame = issc_Flipped ? FlipImage(sc_duckFrames.Last()) : sc_duckFrames.Last();
            scorpionBox.Image = finalDuckFrame;

            isScDuckFlipping = false;
        }
        private async Task sc_StartMoveSequence()
        {
            if (!isRoundActive) return;

            sc_currentState = ScorpionState.Moving;
            frameIndex = 0;

            while (isAKeyPressed == true || isDKeyPressed == true)
            {
                Rectangle scHitbox = scorpionBox.Bounds;
                Rectangle szHitbox = SubzeroBox.Bounds;

                // If overlapping, push both back
                if (scHitbox.IntersectsWith(szHitbox))
                {
                    if (scorpionBox.Left < SubzeroBox.Left)
                    {
                        scorpionBox.Left -= 5;
                        SubzeroBox.Left += 5;
                    }
                    else
                    {
                        scorpionBox.Left += 5;
                        SubzeroBox.Left -= 5;
                    }

                    // Clamp inside screen
                    scorpionBox.Left = Math.Max(0, Math.Min(this.ClientSize.Width - scorpionBox.Width, scorpionBox.Left));
                    SubzeroBox.Left = Math.Max(0, Math.Min(this.ClientSize.Width - SubzeroBox.Width, SubzeroBox.Left));

                    continue; // Skip movement for this frame
                }

                int dir = sc_GetMoveDirection();
                // Determine if we're moving forward
                bool isForward = false;
                if (issc_Flipped == false && dir == 1) { isForward = true; }
                if (issc_Flipped == true && dir == -1) { isForward = true; }
                if (issc_SprintActive && isForward == true)
                {
                    sc_sprintSound.Play();
                }
                // Sprint only works when moving forward
                bool useSprint = issc_SprintActive == true && isForward == true;
                List<Image> frames = useSprint ? sc_sprintFrames : sc_moveFrames;
                int moveStep = useSprint ? 40 : 10;
                int delay = useSprint ? 35 : 60;


                // Choose correct animation frame
                Image frame;
                if (isForward == true)
                {
                    frame = frames[frameIndex % frames.Count];
                }
                else
                {
                    int reverseIndex = frames.Count - 1 - (frameIndex % frames.Count);
                    frame = frames[reverseIndex];
                }

                // Display frame
                if (issc_Flipped == true)
                {
                    scorpionBox.Image = FlipImage(frame);
                }
                else
                {
                    scorpionBox.Image = frame;
                }

                // Apply bounded movement
                int newX = scorpionBox.Left + (dir * moveStep);
                if (newX < 0)
                {
                    newX = 0;
                }
                if (newX > this.ClientSize.Width - scorpionBox.Width)
                {
                    newX = this.ClientSize.Width - scorpionBox.Width;
                }

                scorpionBox.Left = newX;

                frameIndex = frameIndex + 1;

                // Always delay to prevent acceleration
                await Task.Delay(delay);
            }

            sc_currentState = ScorpionState.Idle;
            frameIndex = 0;
            UpdateCharacterFacing();

        }
        private async Task sc_PlayFallSequence()
        {
            sc_currentState = ScorpionState.Falling;
            sc_isKnockedOut = true;

            SetScorpionSize(390, 390);

            foreach (Image frame in sc_fallFrames)
            {
                scorpionBox.Image = issc_Flipped ? FlipImage(frame) : frame;
                await Task.Delay(60); // Adjust delay if needed
            }

            // Align and freeze on floor
            scorpionBox.Top = this.ClientSize.Height - 300;
            scorpionBox.Image = issc_Flipped ? FlipImage(sc_fallFrames.Last()) : sc_fallFrames.Last();
            await Task.Delay(500);

            // Clamp X within bounds (optional but safer)
            scorpionBox.Left = Math.Max(100, Math.Min(this.ClientSize.Width, scorpionBox.Left));
        }
        private async Task sc_PlayGetUpSequence()
        {
            sc_currentState = ScorpionState.Gettingup;
            SetScorpionSize(300, 300);

            foreach (Image frame in sc_getUpFrames)
            {
                scorpionBox.Image = issc_Flipped ? FlipImage(frame) : frame;
                await Task.Delay(60);
            }

            scorpionBox.Top = this.ClientSize.Height - 100;
            sc_currentState = ScorpionState.Idle;

            scorpionBox.Left = Math.Max(0, Math.Min(this.ClientSize.Width - scorpionBox.Width, scorpionBox.Left));

        }
        private async Task sc_StartTeleportSequence()
        {
            if (!isRoundActive) return;

            sc_currentState = ScorpionState.teleport;
            SetScorpionSize(400, 400);

            tpGruntSound.Play();

            int teleportYOffset = 130; // Raise teleport height (punch to chest)

            // === Disappear Animation ===
            for (int i = 0; i < sc_tpFrames.Count / 2; i++)
            {
                Image frame = sc_tpFrames[i];
                scorpionBox.Image = issc_Flipped ? FlipImage(frame) : frame;
                await Task.Delay(50);
            }

            // === Exit through border ===
            if (issc_Flipped)
            {
                scorpionBox.Left = -scorpionBox.Width;
            }
            else
            {
                scorpionBox.Left = this.ClientSize.Width;
            }

            await Task.Delay(50);
            try { tpWhipSound.Play(); } catch { }

            // === Reappear behind Sub-Zero (based on his facing) ===
            bool subzeroFacingRight = SubzeroBox.Left > scorpionBox.Left;
            int targetX;

            if (subzeroFacingRight)
            {
                targetX = SubzeroBox.Left - scorpionBox.Width - 10;
                issc_Flipped = false;
            }
            else
            {
                targetX = SubzeroBox.Right + 10;
                issc_Flipped = true;
            }

            scorpionBox.Left = targetX;
            scorpionBox.Top = this.ClientSize.Height - scorpionBox.Height - teleportYOffset;

            // === Reappear Animation ===
            for (int i = sc_tpFrames.Count / 2; i < sc_tpFrames.Count; i++)
            {
                Image frame = sc_tpFrames[i];
                scorpionBox.Image = issc_Flipped ? FlipImage(frame) : frame;
                await Task.Delay(70);
            }
            // === Damage Sub-Zero ===
            DamagePlayer(false, 20);
            CheckForKO();
            // === Sub-Zero Fall + Clamp ===
            await sz_PlayFallSequence();

            if (SubzeroBox.Left < 0)
            {
                SubzeroBox.Left = 0;
            }
            if (SubzeroBox.Left > this.ClientSize.Width - SubzeroBox.Width)
            {
                SubzeroBox.Left = this.ClientSize.Width - SubzeroBox.Width;
            }

            // === Reset Scorpion ===
            sc_currentState = ScorpionState.Idle;
            SetScorpionSize(400, 400);
            scorpionBox.Top = this.ClientSize.Height - scorpionBox.Height;
            UpdateCharacterFacing();
            // === Now Get Up Sub-Zero ===
            if (currentHealthR > 0)
            {
                await sz_PlayGetUpSequence();
                SetSubzeroSize(400, 400);
            }
            await Task.Delay(60);
        }

        private async Task sc_victorypose()
        {
            await sz_PlayFallSequence();
            isRoundActive = false;
            sc_currentState = ScorpionState.victory;
            SetScorpionSize(450, 450);
            foreach (Image frame in sc_victoryFrames)
            {
                scorpionBox.Image = issc_Flipped ? FlipImage(frame) : frame;
                await Task.Delay(60);
            }

            await Task.Delay(500);

            // Clamp just in case
            scorpionBox.Left = Math.Max(0, Math.Min(this.ClientSize.Width - scorpionBox.Width, scorpionBox.Left));
        }

        private async Task sc_StartFrontFlip()
        {
            if (!isRoundActive) return;

            sc_currentState = ScorpionState.Jumping;
            SetScorpionSize(225, 225);
            scorpionBox.SizeMode = PictureBoxSizeMode.Zoom;

            int floorY = this.ClientSize.Height - scorpionBox.Height;
            int offsetX = issc_Flipped ? -60 : 60;

            // Takeoff frame: f01
            scorpionBox.Image = issc_Flipped ? FlipImage(sc_diagJumpFrames[0]) : sc_diagJumpFrames[0];
            scorpionBox.Top = floorY - 320; // initial lift
            await Task.Delay(60);

            // Mid-air: f02 to f08 × 2 cycles
            for (int loop = 0; loop < 1; loop++)
            {
                for (int i = 1; i <= 7; i++)
                {
                    Image frame = sc_diagJumpFrames[i];
                    scorpionBox.Image = issc_Flipped ? FlipImage(frame) : frame;

                    // Slight arc
                    scorpionBox.Top -= 30;
                    scorpionBox.Left += offsetX / 3;

                    await Task.Delay(60);
                }
            }

            // Descent: f02 to f08 again to keep flipping forward
            for (int i = 1; i <= 7; i++)
            {
                Image frame = sc_diagJumpFrames[i];
                scorpionBox.Image = issc_Flipped ? FlipImage(frame) : frame;

                scorpionBox.Top += 70;

                if (scorpionBox.Top > floorY)
                {
                    scorpionBox.Top = floorY;
                }

                scorpionBox.Left += offsetX / 3;
                if (scorpionBox.Left < 0) scorpionBox.Left = 0;
                if (scorpionBox.Left > this.ClientSize.Width - scorpionBox.Width)
                {
                    scorpionBox.Left = this.ClientSize.Width - scorpionBox.Width;
                }

                await Task.Delay(60);
            }

            // Land with f01
            scorpionBox.Image = issc_Flipped ? FlipImage(sc_diagJumpFrames[0]) : sc_diagJumpFrames[0];
            scorpionBox.Top = floorY;
            await Task.Delay(60);

            SetScorpionSize(400, 400);
            sc_currentState = ScorpionState.Idle;
            frameIndex = 0;
            UpdateCharacterFacing();
        }
        private async Task sc_StartBackFlip()
        {
            if (!isRoundActive) return;

            sc_currentState = ScorpionState.Jumping;
            SetScorpionSize(225, 225);
            scorpionBox.SizeMode = PictureBoxSizeMode.Zoom;

            int floorY = this.ClientSize.Height - scorpionBox.Height;
            int offsetX = issc_Flipped ? 60 : -60;

            // Takeoff frame: f01
            scorpionBox.Image = issc_Flipped ? FlipImage(sc_diagJumpFrames[0]) : sc_diagJumpFrames[0];
            scorpionBox.Top = floorY - 320; // lift off
            await Task.Delay(60);

            // Mid-air: f08 to f02 × 2 cycles
            for (int loop = 0; loop < 1; loop++)
            {
                for (int i = 7; i >= 1; i--)
                {
                    Image frame = sc_diagJumpFrames[i];
                    scorpionBox.Image = issc_Flipped ? FlipImage(frame) : frame;

                    scorpionBox.Top -= 30;
                    scorpionBox.Left += offsetX / 3;

                    await Task.Delay(60);
                }
            }
            // Descent: f08 to f02 again
            for (int i = 7; i >= 1; i--)
            {
                Image frame = sc_diagJumpFrames[i];
                scorpionBox.Image = issc_Flipped ? FlipImage(frame) : frame;

                scorpionBox.Top += 70;

                if (scorpionBox.Top > floorY)
                {
                    scorpionBox.Top = floorY;
                }

                scorpionBox.Left += offsetX / 3;
                if (scorpionBox.Left < 0) scorpionBox.Left = 0;
                if (scorpionBox.Left > this.ClientSize.Width - scorpionBox.Width)
                {
                    scorpionBox.Left = this.ClientSize.Width - scorpionBox.Width;
                }

                await Task.Delay(60);
            }
            // Land with f01
            scorpionBox.Image = issc_Flipped ? FlipImage(sc_diagJumpFrames[0]) : sc_diagJumpFrames[0];
            scorpionBox.Top = floorY;
            await Task.Delay(60);

            SetScorpionSize(400, 400);
            sc_currentState = ScorpionState.Idle;
            frameIndex = 0;
            UpdateCharacterFacing();
        }
        private async Task sc_StartJumpSequence()
        {
            if (!isRoundActive) return;

            sc_currentState = ScorpionState.Jumping;
            SetScorpionSize(290, 290);
            scorpionBox.Top = this.ClientSize.Height - scorpionBox.Height;

            // Jump upward frames
            for (int i = 0; i <= 1; i++)
            {
                scorpionBox.Image = issc_Flipped ? FlipImage(sc_jumpFrames[i]) : sc_jumpFrames[i];
                scorpionBox.Top -= 300;

                await Task.Delay(i == 1 ? 200 : 75); // Pause on second frame
            }

            // Descend frames (reverse)
            for (int i = 2; i >= 0; i--)
            {
                scorpionBox.Image = issc_Flipped ? FlipImage(sc_jumpFrames[i]) : sc_jumpFrames[i];
                scorpionBox.Top += 250;

                if (scorpionBox.Top > this.ClientSize.Height - scorpionBox.Height)
                    scorpionBox.Top = this.ClientSize.Height - scorpionBox.Height;

                await Task.Delay(90);
            }

            await sc_PlayLandingSequence();
        }

        private async Task sc_PlayLandingSequence()
        {
            if (issc_Flipped == true)
            {
                scorpionBox.Image = FlipImage(sc_idleFrames[1]);
            }
            else
            {
                scorpionBox.Image = sc_idleFrames[1];
            }

            await Task.Delay(70);

            if (issc_Flipped == true)
            {
                scorpionBox.Image = FlipImage(sc_idleFrames[0]);
            }
            else
            {
                scorpionBox.Image = sc_idleFrames[0];
            }

            await Task.Delay(30);
            SetScorpionSize(400, 400);
            sc_currentState = ScorpionState.Idle;
            frameIndex = 0;
            UpdateCharacterFacing();
        }
        private async Task sc_StartThrowSequence()
        {
            sc_currentState = ScorpionState.Throwing;
            frameIndex = 0;
            sc_PlayFrames(sc_throwFrames);

            await sc_LaunchRope();
            sc_PlayRandomVoiceLine();

            sc_currentState = ScorpionState.Pulling;
            frameIndex = 0;
            await sc_ReturnRope();
            ropeBox.Visible = false;
            sc_currentState = ScorpionState.Idle;
            frameIndex = 0;
        }
        private async Task sc_LaunchRope()
        {
            int ropeY = scorpionBox.Top + scorpionBox.Height / 8;
            string ropePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Characters", "Scorpion", "Sprites", "rope.png");

            bool launchFlipped = issc_Flipped;

            if (launchFlipped)
            {
                ropeX = scorpionBox.Right - 10;
            }
            else
            {
                ropeX = scorpionBox.Left - ropeBox.Width + 10;
            }

            Image ropeImage = Image.FromFile(ropePath);
            if (launchFlipped)
            {
                ropeBox.Image = FlipImage(ropeImage);
            }
            else
            {
                ropeBox.Image = ropeImage;
            }

            ropeBox.Location = new Point(ropeX, ropeY);
            ropeBox.Visible = true;

            try { ropeSound.Play(); } catch { }

            int dx;
            if (launchFlipped)
            {
                dx = -80;
            }
            else
            {
                dx = 80;
            }

            int scFrontXAtHit;
            if (launchFlipped)
            {
                scFrontXAtHit = scorpionBox.Right;
            }
            else
            {
                scFrontXAtHit = scorpionBox.Left;
            }

            while ((launchFlipped && ropeBox.Left > 0) || (!launchFlipped && ropeBox.Right < this.ClientSize.Width))
            {
                ropeX += dx;
                ropeBox.Location = new Point(ropeX, ropeY);

                Rectangle ropeHitbox = ropeBox.Bounds;
                Rectangle szHitbox = SubzeroBox.Bounds;

                if (ropeHitbox.IntersectsWith(szHitbox))
                {
                    DamagePlayer(false, 20);
                    CheckForKO();
                    sc_currentState = ScorpionState.Pulling;
                    sc_PlayFrames(sc_pullFrames);

                    await sc_ReturnRope();
                    break;
                }

                await Task.Delay(15);
            }

            ropeBox.Visible = false;
        }
        private async Task sc_ReturnRope()
        {
            int ropeY = ropeBox.Top;
            bool returnFlipped = issc_Flipped;

            int targetX;
            if (returnFlipped)
            {
                targetX = scorpionBox.Left - ropeBox.Width + 10;
            }
            else
            {
                targetX = scorpionBox.Left + scorpionBox.Width - 10;
            }

            int dx;
            if (returnFlipped)
            {
                dx = 70;
            }
            else
            {
                dx = -70;
            }

            string ropePullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Characters", "Scorpion", "Sprites", "rope_pull.png");
            Image ropePullImage = Image.FromFile(ropePullPath);
            if (returnFlipped)
            {
                ropeBox.Image = FlipImage(ropePullImage);
            }
            else
            {
                ropeBox.Image = ropePullImage;
            }

            string pullFramePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Characters", "Subzero", "Sprites", "rope_pull.png");
            Image pullFrame = Image.FromFile(pullFramePath);
            if (issz_Flipped)
            {
                SubzeroBox.Image = FlipImage(pullFrame);
            }
            else
            {
                SubzeroBox.Image = pullFrame;
            }

            sz_currentState = SubzeroState.Moving;

            while ((returnFlipped && ropeX < targetX) || (!returnFlipped && ropeX > targetX))
            {
                ropeX += dx;
                ropeBox.Location = new Point(ropeX, ropeY);

                int scFront;
                if (returnFlipped)
                {
                    scFront = scorpionBox.Left;
                }
                else
                {
                    scFront = scorpionBox.Right;
                }

                int diff = scFront - SubzeroBox.Left;

                int pullStep = 0;
                if (diff > 80)
                {
                    pullStep = 80;
                }
                else if (diff < -80)
                {
                    pullStep = -80;
                }
                else
                {
                    pullStep = diff;
                }

                SubzeroBox.Left += pullStep;

                if (SubzeroBox.Left < 0)
                {
                    SubzeroBox.Left = 0;
                }

                if (SubzeroBox.Left > this.ClientSize.Width - SubzeroBox.Width)
                {
                    SubzeroBox.Left = this.ClientSize.Width - SubzeroBox.Width;
                }
                await Task.Delay(20);
            }

            // Snap once at the end to avoid jitter
            int snapX;
            int buffer = 10;

            if (returnFlipped)
            {
                snapX = scorpionBox.Left - SubzeroBox.Width - buffer;
                if (snapX < 0) snapX = 0;
            }
            else
            {
                snapX = scorpionBox.Right + buffer;
                if (snapX > this.ClientSize.Width - SubzeroBox.Width)
                    snapX = this.ClientSize.Width - SubzeroBox.Width;
            }

            SubzeroBox.Left = snapX;

            ropeBox.Visible = false;
            if (currentHealthR > 0)
            {
                SetSubzeroSize(400, 400);
                sz_currentState = SubzeroState.Idle;
                sz_frameIndex = 0;
            }
        }

        private void sc_PlayFrames(List<Image> frames)
        {
            foreach (Image frame in frames)
            {
                if (issc_Flipped == true)
                {
                    scorpionBox.Image = FlipImage(frame);
                }
                else
                {
                    scorpionBox.Image = frame;
                }

                Task.Delay(60).Wait();
            }
        }
        private int sc_GetMoveDirection()
        {
            if (isAKeyPressed == true && isDKeyPressed == false) { return -1; }
            if (isDKeyPressed == true && isAKeyPressed == false) { return 1; }
            return 0;
        }
        private void sc_PlayRandomVoiceLine()
        {
            try
            {
                int choice = rand.Next(2);
                if (choice == 0)
                {
                    getOverHereSound.Play();
                }
                else
                {
                    comeHereSound.Play();
                }
            }
            catch { }
        }
        //end of scorpion's functionalities
        private Image FlipImage(Image img)
        {
            Bitmap flipped = new Bitmap(img);
            flipped.RotateFlip(RotateFlipType.RotateNoneFlipX);
            return flipped;
        }
        //subzero functionalities
        private async void sz_MainForm_KeyDown(object sender, KeyEventArgs e)//to merge
        {
            if (!isRoundActive) return;

            if (e.KeyCode == Keys.M)
            {
                issz_SprintActive = !issz_SprintActive;
            }

            if (e.KeyCode == Keys.F && sz_currentState == SubzeroState.Idle)
                await sz_StartFreezeSequence();

            if (e.KeyCode == Keys.N && sz_currentState == SubzeroState.Idle)
                await sz_StartSlideSequence();

            if (e.KeyCode == Keys.Up) isupPressed = true;
            if (e.KeyCode == Keys.Left) isleftPressed = true;
            if (e.KeyCode == Keys.Right) isrightPressed = true;

            if (e.KeyCode == Keys.Down)
            {
                issz_DuckPressed = true;

                if (sz_currentState == SubzeroState.Idle || sz_currentState == SubzeroState.Moving)
                {
                    _ = sz_StartDuckSequence();
                }
            }

            if ((e.KeyCode == Keys.Left || e.KeyCode == Keys.Right) && sz_currentState == SubzeroState.Idle)
                await sz_StartMoveSequence();

            if (e.KeyCode == Keys.I && sz_currentState != SubzeroState.Jumping && sz_currentState != SubzeroState.Ducking)
            {
                if (issz_Flipped) await sz_StartFrontFlip();
                else await sz_StartBackFlip();
            }

            if (e.KeyCode == Keys.O && sz_currentState != SubzeroState.Jumping && sz_currentState != SubzeroState.Ducking)
            {
                if (issz_Flipped) await sz_StartBackFlip();
                else await sz_StartFrontFlip();
            }

            if (e.KeyCode == Keys.Up && sz_currentState != SubzeroState.Jumping && sz_currentState != SubzeroState.Ducking)
                await sz_StartJumpSequence();

            if (e.KeyCode == Keys.Escape)
                Application.Exit();
        }

        private void sz_MainForm_KeyUp(object sender, KeyEventArgs e)//to merge
        {
            if (e.KeyCode == Keys.Left) isleftPressed = false;
            if (e.KeyCode == Keys.Right) isrightPressed = false;
            if (e.KeyCode == Keys.Up) isupPressed = false;
            if (e.KeyCode == Keys.Down)
            {
                issz_DuckPressed = false;

                if (sz_currentState == SubzeroState.Ducking)
                {
                    _ = sz_PlayStandFromDuck();
                }
            }
        }
        private async Task sz_StartFreezeSequence()//to merge
        {
            if (!isRoundActive) return;

            sz_freezeStartSound.Play();
            sz_currentState = SubzeroState.Freeze;
            SetSubzeroSize(300, 300);

            foreach (var frame in sz_freezeFrames)
            {
                SubzeroBox.Image = issz_Flipped ? FlipImage(frame) : frame;
                await Task.Delay(60);
            }

            _ = sz_LaunchIceProjectile();

            for (int i = sz_freezeFrames.Count - 1; i >= 0; i--)
            {
                SubzeroBox.Image = issz_Flipped ? FlipImage(sz_freezeFrames[i]) : sz_freezeFrames[i];
                await Task.Delay(60);
            }

            SetSubzeroSize(400, 400);
            sz_currentState = SubzeroState.Idle;
            sz_frameIndex = 0;
        }
        private bool isIceActive = false;

        private async Task sz_LaunchIceProjectile()
        {
            if (isIceActive) return; // Prevent multiple projectiles
            isIceActive = true;

            sz_iceLaunchSound.Play();

            iceBox.Image = issz_Flipped ? FlipImage(sz_pushFrames[0]) : sz_pushFrames[0];
            iceBox.Visible = true;

            int sz_startY = SubzeroBox.Top + SubzeroBox.Height / 30;
            int sz_startX = issz_Flipped ? SubzeroBox.Left : SubzeroBox.Right;
            iceBox.Location = new Point(sz_startX, sz_startY);

            int sz_dx = issz_Flipped ? -50 : 50;

            while (iceBox.Right > 0 && iceBox.Left < this.ClientSize.Width)
            {
                for (int i = 0; i < sz_pushFrames.Count; i++)
                {
                    Image frame = sz_pushFrames[i];
                    iceBox.Image = issz_Flipped ? FlipImage(frame) : frame;

                    // Move projectile
                    int sz_newX = iceBox.Left + sz_dx;
                    iceBox.Left = sz_newX;

                    // Collision with Scorpion
                    Rectangle iceHitbox = iceBox.Bounds;
                    Rectangle scHitbox = scorpionBox.Bounds;

                    if (iceHitbox.IntersectsWith(scHitbox))
                    {
                        // Play hit frames
                        for (int j = 0; j < sz_hitFrames.Count; j++)
                        {
                            iceBox.Image = issz_Flipped ? FlipImage(sz_hitFrames[j]) : sz_hitFrames[j];

                        }
                        // Freeze Scorpion
                        string frozenPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Characters", "Scorpion", "Sprites", "being_hit", "frozen", "frozen.png");
                        Image frozenImage = Image.FromFile(frozenPath);
                        scorpionBox.Image = issc_Flipped ? FlipImage(frozenImage) : frozenImage;
                        sc_currentState = ScorpionState.Ducking; // Prevent movement temporarily

                        iceBox.Visible = false;
                        isIceActive = false;
                        DamagePlayer(true, 15);
                        CheckForKO();
                        if (sc_isKnockedOut) return;
                        await Task.Delay(2800); // Frozen duration
                        if (currentHealthL > 0)
                        {
                            sc_currentState = ScorpionState.Idle;
                            SetScorpionSize(400, 400);
                            return;
                        }
                    }
                    await Task.Delay(25);
                }
            }

            // Play impact frames if no hit
            for (int i = 0; i < sz_hitFrames.Count; i++)
            {
                Image frame = sz_hitFrames[i];
                iceBox.Image = issz_Flipped ? FlipImage(frame) : frame;
                await Task.Delay(60);
            }

            iceBox.Visible = false;
            isIceActive = false;
            SetSubzeroSize(400, 400); // Restore size
        }

        private async Task sz_StartSlideSequence()
        {
            if (!isRoundActive) return;

            sz_currentState = SubzeroState.Sliding;
            SetSubzeroSize(300, 300);
            SubzeroBox.Top = sz_floorY + 110;
            sz_slideSound.Play();

            int sz_step = issz_Flipped ? -150 : 150;
            int sz_targetX = issz_Flipped ? 0 : this.ClientSize.Width - SubzeroBox.Width;

            Image sz_slideFrame = sz_slideFrames.Count >= 2 ? sz_slideFrames[1] : sz_slideFrames[0];
            SubzeroBox.Image = issz_Flipped ? FlipImage(sz_slideFrame) : sz_slideFrame;

            while ((issz_Flipped && SubzeroBox.Left > sz_targetX) || (!issz_Flipped && SubzeroBox.Left < sz_targetX))
            {
                SubzeroBox.Left += sz_step;
                SubzeroBox.Left = Math.Max(0, Math.Min(this.ClientSize.Width - SubzeroBox.Width, SubzeroBox.Left));

                Rectangle szHitbox = SubzeroBox.Bounds;
                Rectangle scHitbox = scorpionBox.Bounds;

                if (szHitbox.IntersectsWith(scHitbox))
                {
                    DamagePlayer(true, 30);

                    int pushAmount = 30;
                    if (issz_Flipped)
                    {
                        SubzeroBox.Left = Math.Max(0, SubzeroBox.Left + pushAmount);
                        scorpionBox.Left = Math.Min(this.ClientSize.Width - scorpionBox.Width, scorpionBox.Left - pushAmount);
                    }
                    else
                    {
                        SubzeroBox.Left = Math.Min(this.ClientSize.Width - SubzeroBox.Width, SubzeroBox.Left - pushAmount);
                        scorpionBox.Left = Math.Max(0, scorpionBox.Left + pushAmount);
                    }

                    if (isScorpionFrozen) isScorpionFrozen = false;

                    // Immediately end slide state for Sub-Zero
                    sz_currentState = SubzeroState.Idle;
                    SubzeroBox.Top = sz_floorY + 280;
                    SetSubzeroSize(400, 400);
                    sz_frameIndex = 0;
                    CheckForKO();
                    // Only let Scorpion get up if not KO’ed
                    sc_currentState = ScorpionState.Falling;
                    await sc_PlayFallSequence();
                    await Task.Delay(600);

                    if (currentHealthL > 0)
                    {
                        await sc_PlayGetUpSequence();
                        SetScorpionSize(400, 400);
                        sc_currentState = ScorpionState.Idle;
                    }

                    return;
                }

                await Task.Delay(60);
            }

            // Slide missed — animate exit
            for (int i = sz_slideFrames.Count - 1; i >= 0; i--)
            {
                SubzeroBox.Image = issz_Flipped ? FlipImage(sz_slideFrames[i]) : sz_slideFrames[i];
                await Task.Delay(20);
            }

            SubzeroBox.Top = sz_floorY + 280;
            sz_currentState = SubzeroState.Idle;
            SetSubzeroSize(400, 400);
            sz_frameIndex = 0;
            UpdateCharacterFacing();
        }


        private async Task sz_StartMoveSequence()//to merge
        {
            if (!isRoundActive) return;

            sz_currentState = SubzeroState.Moving;
            sz_frameIndex = 0;

            while (isleftPressed || isrightPressed)
            {
                // Define hitboxes before movement
                Rectangle scHitbox = scorpionBox.Bounds;
                Rectangle szHitbox = SubzeroBox.Bounds;
                // If overlapping, push both back
                if (scHitbox.IntersectsWith(szHitbox))
                {
                    if (scorpionBox.Left < SubzeroBox.Left)
                    {
                        scorpionBox.Left -= 5;
                        SubzeroBox.Left += 5;
                    }
                    else
                    {
                        scorpionBox.Left += 5;
                        SubzeroBox.Left -= 5;
                    }

                    // Clamp inside screen
                    scorpionBox.Left = Math.Max(0, Math.Min(this.ClientSize.Width - scorpionBox.Width, scorpionBox.Left));
                    SubzeroBox.Left = Math.Max(0, Math.Min(this.ClientSize.Width - SubzeroBox.Width, SubzeroBox.Left));

                    continue; // Skip movement for this frame
                }


                int dir = sz_GetMoveDirection();
                bool issz_Forward = (issz_Flipped == false && dir == 1) || (issz_Flipped == true && dir == -1);
                bool sz_useSprint = issz_SprintActive && issz_Forward;
                if (issz_SprintActive && issz_Forward == true)
                {
                    sz_sprintSound.Play();
                }
                List<Image> frames = sz_useSprint ? sz_sprintFrames : sz_moveFrames;
                int sz_moveStep = sz_useSprint ? 40 : 10;
                int sz_delay = sz_useSprint ? 35 : 60;

                Image sz_frame = issz_Forward
                    ? frames[sz_frameIndex % frames.Count]
                    : frames[frames.Count - 1 - (sz_frameIndex % frames.Count)];

                SubzeroBox.Image = issz_Flipped ? FlipImage(sz_frame) : sz_frame;

                int sz_newX = SubzeroBox.Left + (dir * sz_moveStep);
                sz_newX = Math.Max(0, Math.Min(this.ClientSize.Width - SubzeroBox.Width, sz_newX));
                SubzeroBox.Left = sz_newX;
                sz_frameIndex++;
                await Task.Delay(sz_delay);
            }

            sz_currentState = SubzeroState.Idle;
            sz_frameIndex = 0;
            UpdateCharacterFacing();
        }

        private async Task sz_StartJumpSequence()
        {
            if (!isRoundActive) return;

            sz_currentState = SubzeroState.Jumping;
            SetSubzeroSize(290, 290);
            SubzeroBox.Top = this.ClientSize.Height - SubzeroBox.Height;

            // Jump upward frames
            for (int i = 0; i <= 1; i++)
            {
                SubzeroBox.Image = issz_Flipped ? FlipImage(sz_jumpFrames[i]) : sz_jumpFrames[i];
                SubzeroBox.Top -= 300;

                await Task.Delay(i == 1 ? 200 : 75); // Pause on second frame
            }

            // Descend frames (reverse)
            for (int i = 2; i >= 0; i--)
            {
                SubzeroBox.Image = issz_Flipped ? FlipImage(sz_jumpFrames[i]) : sz_jumpFrames[i];
                SubzeroBox.Top += 250;

                if (SubzeroBox.Top > this.ClientSize.Height - SubzeroBox.Height)
                    SubzeroBox.Top = this.ClientSize.Height - SubzeroBox.Height;

                await Task.Delay(90);
            }

            await sz_PlayLandingSequence();
        }

        private async Task sz_StartFrontFlip()//to merge
        {
            if (!isRoundActive) return;

            sz_currentState = SubzeroState.Jumping;
            SetSubzeroSize(225, 225);
            int sz_floorY = this.ClientSize.Height - SubzeroBox.Height;
            int sz_offsetX = issz_Flipped ? -60 : 60;

            SubzeroBox.Image = issz_Flipped ? FlipImage(sz_diagJumpFrames[0]) : sz_diagJumpFrames[0];
            SubzeroBox.Top = sz_floorY - 320;
            await Task.Delay(60);

            for (int i = 1; i <= 7; i++)
            {
                SubzeroBox.Image = issz_Flipped ? FlipImage(sz_diagJumpFrames[i]) : sz_diagJumpFrames[i];
                SubzeroBox.Top -= 30;
                SubzeroBox.Left += sz_offsetX / 3;

                if (SubzeroBox.Left < 0) SubzeroBox.Left = 0;
                if (SubzeroBox.Left > this.ClientSize.Width - SubzeroBox.Width)
                    SubzeroBox.Left = this.ClientSize.Width - SubzeroBox.Width;

                await Task.Delay(60);
            }

            for (int i = 1; i <= 7; i++)
            {
                SubzeroBox.Image = issz_Flipped ? FlipImage(sz_diagJumpFrames[i]) : sz_diagJumpFrames[i];
                SubzeroBox.Top += 70;
                if (SubzeroBox.Top > sz_floorY) SubzeroBox.Top = sz_floorY;

                SubzeroBox.Left += sz_offsetX / 3;

                if (SubzeroBox.Left < 0) SubzeroBox.Left = 0;
                if (SubzeroBox.Left > this.ClientSize.Width - SubzeroBox.Width)
                    SubzeroBox.Left = this.ClientSize.Width - SubzeroBox.Width;

                await Task.Delay(60);
            }

            SubzeroBox.Image = issz_Flipped ? FlipImage(sz_diagJumpFrames[0]) : sz_diagJumpFrames[0];
            SubzeroBox.Top = sz_floorY;
            await Task.Delay(60);
            SetSubzeroSize(400, 400);
            sz_currentState = SubzeroState.Idle;
            sz_frameIndex = 0;
            UpdateCharacterFacing();
        }

        private async Task sz_StartBackFlip()//to merge
        {
            if (!isRoundActive) return;

            sz_currentState = SubzeroState.Jumping;
            SetSubzeroSize(225, 225);
            int sz_floorY = this.ClientSize.Height - SubzeroBox.Height;
            int sz_offsetX = issz_Flipped ? 60 : -60;

            SubzeroBox.Image = issz_Flipped ? FlipImage(sz_diagJumpFrames[0]) : sz_diagJumpFrames[0];
            SubzeroBox.Top = sz_floorY - 320;
            await Task.Delay(60);

            for (int i = 7; i >= 1; i--)
            {
                SubzeroBox.Image = issz_Flipped ? FlipImage(sz_diagJumpFrames[i]) : sz_diagJumpFrames[i];
                SubzeroBox.Top -= 30;
                SubzeroBox.Left += sz_offsetX / 3;

                if (SubzeroBox.Left < 0) SubzeroBox.Left = 0;
                if (SubzeroBox.Left > this.ClientSize.Width - SubzeroBox.Width)
                    SubzeroBox.Left = this.ClientSize.Width - SubzeroBox.Width;

                await Task.Delay(60);
            }

            for (int i = 7; i >= 1; i--)
            {
                SubzeroBox.Image = issz_Flipped ? FlipImage(sz_diagJumpFrames[i]) : sz_diagJumpFrames[i];
                SubzeroBox.Top += 70;
                if (SubzeroBox.Top > sz_floorY) SubzeroBox.Top = sz_floorY;

                SubzeroBox.Left += sz_offsetX / 3;

                if (SubzeroBox.Left < 0) SubzeroBox.Left = 0;
                if (SubzeroBox.Left > this.ClientSize.Width - SubzeroBox.Width)
                    SubzeroBox.Left = this.ClientSize.Width - SubzeroBox.Width;

                await Task.Delay(60);
            }

            SubzeroBox.Image = issz_Flipped ? FlipImage(sz_diagJumpFrames[0]) : sz_diagJumpFrames[0];
            SubzeroBox.Top = sz_floorY;
            await Task.Delay(60);
            SetSubzeroSize(400, 400);
            sz_currentState = SubzeroState.Idle;
            sz_frameIndex = 0;
            UpdateCharacterFacing();
        }

        private async Task sz_StartDuckSequence()//to merge
        {
            if (!isRoundActive) return;

            sz_currentState = SubzeroState.Ducking;
            SetSubzeroSize(220, 220);

            foreach (Image sz_frame in sz_duckFrames)
            {
                SubzeroBox.Image = issz_Flipped ? FlipImage(sz_frame) : sz_frame;
                await Task.Delay(70);
                UpdateCharacterFacing();
            }

            Image sz_lastFrame = sz_duckFrames.Last();
            SubzeroBox.Image = issz_Flipped ? FlipImage(sz_lastFrame) : sz_lastFrame;
        }

        private async Task sz_PlayStandFromDuck()
        {
            if (!isRoundActive || sz_currentState != SubzeroState.Ducking)
                return;

            sz_currentState = SubzeroState.Moving; // Temporary transition state

            for (int i = sz_duckFrames.Count - 1; i >= 0; i--)
            {
                Image sz_frame = sz_duckFrames[i];
                SubzeroBox.Image = issz_Flipped ? FlipImage(sz_frame) : sz_frame;
                await Task.Delay(70);
            }

            SetSubzeroSize(400, 400);
            SubzeroBox.Top = sz_floorY; // Ensure he's grounded properly

            sz_currentState = SubzeroState.Idle;
            sz_frameIndex = 0;

            UpdateCharacterFacing(); // Flip to face opponent
        }
        private async Task sz_PlayDuckStanceFlip()
        {
            if (isSzDuckFlipping) return;

            isSzDuckFlipping = true;
            sz_currentState = SubzeroState.Ducking;

            for (int i = 0; i < sz_DstanceFrames.Count; i++)
            {
                SubzeroBox.Image = issz_Flipped ? FlipImage(sz_DstanceFrames[i]) : sz_DstanceFrames[i];
                await Task.Delay(70);
            }

            issz_Flipped = !issz_Flipped;

            Image finalDuckFrame = issz_Flipped ? FlipImage(sz_duckFrames.Last()) : sz_duckFrames.Last();
            SubzeroBox.Image = finalDuckFrame;

            isSzDuckFlipping = false;
        }

        private async Task sz_PlayLandingSequence()//to merge
        {
            SubzeroBox.Image = issz_Flipped ? FlipImage(sz_idleFrames[1]) : sz_idleFrames[1];
            await Task.Delay(70);
            SubzeroBox.Image = issz_Flipped ? FlipImage(sz_idleFrames[0]) : sz_idleFrames[0];
            await Task.Delay(30);
            SetSubzeroSize(400, 400);
            sz_currentState = SubzeroState.Idle;
            sz_frameIndex = 0;
            UpdateCharacterFacing();
        }

        private void SetSubzeroSize(int width, int height)//to merge
        {
            int bottom = SubzeroBox.Top + SubzeroBox.Height;
            SubzeroBox.Size = new Size(width, height);
            SubzeroBox.Top = bottom - SubzeroBox.Height;
            int floorY = this.ClientSize.Height - SubzeroBox.Height;
            if (SubzeroBox.Top > floorY)
            {
                SubzeroBox.Top = floorY;
            }
        }
        private int sz_GetMoveDirection()//to merge
        {
            if (isleftPressed && !isrightPressed) return -1;
            if (isrightPressed && !isleftPressed) return 1;
            return 0;
        }

        private async Task sz_PlayFallSequence()
        {
            sz_currentState = SubzeroState.Falling;
            sz_isKnockedOut = true; // Lock his KO state

            SetSubzeroSize(390, 390);

            foreach (Image frame in sz_fallFrames)
            {
                SubzeroBox.Image = issz_Flipped ? FlipImage(frame) : frame;
                await Task.Delay(60);
            }

            // Align and stay on floor
            SubzeroBox.Top = this.ClientSize.Height - 300;
            SubzeroBox.Image = issz_Flipped ? FlipImage(sz_fallFrames.Last()) : sz_fallFrames.Last();
            await Task.Delay(500);

            SubzeroBox.Left = Math.Max(100, Math.Min(this.ClientSize.Width, SubzeroBox.Left));
        }


        private async Task sz_PlayGetUpSequence()
        {
            sz_currentState = SubzeroState.Gettingup;
            SetSubzeroSize(300, 300);

            foreach (Image frame in sz_getUpFrames)
            {
                SubzeroBox.Image = issz_Flipped ? FlipImage(frame) : frame;
                await Task.Delay(60);
            }

            SubzeroBox.Top = this.ClientSize.Height - 100;
            sz_currentState = SubzeroState.Idle;

            SubzeroBox.Left = Math.Max(0, Math.Min(this.ClientSize.Width - SubzeroBox.Width, SubzeroBox.Left));
        }

        private async Task sz_victorypose()
        {
            await sc_PlayFallSequence();
            isRoundActive = false;
            sz_currentState = SubzeroState.victory;
            SetSubzeroSize(450, 450);
            foreach (Image frame in sz_victoryFrames)
            {
                SubzeroBox.Image = issc_Flipped ? FlipImage(frame) : frame;
                await Task.Delay(60);
            }

            // Hold final frame
            await Task.Delay(500);

            // Clamp just in case
            SubzeroBox.Left = Math.Max(0, Math.Min(this.ClientSize.Width - SubzeroBox.Width, SubzeroBox.Left));
        }
    }
}       //end of subzero functionalities