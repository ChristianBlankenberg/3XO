namespace _3XO
{
    using GameLogic;
    using TicTacToe.GameLogic;

    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private Button[,] buttons = new Button[3, 3];
        private Game game;

        private void InitButtons()
        {
            //this.buttons[0, 0] = (Button)this.FindViewById(Resource.Id.button00);
            //this.buttons[0, 0].Click += (object? sender, EventArgs e) => this.ButtonClick(this.buttons[0, 0], 0, 0);
            //this.buttons[1, 0] = (Button)this.FindViewById(Resource.Id.button10);
            //this.buttons[1, 0].Click += (object? sender, EventArgs e) => this.ButtonClick(this.buttons[1, 0], 1, 0);
            //this.buttons[2, 0] = (Button)this.FindViewById(Resource.Id.button20);
            //this.buttons[2, 0].Click += (object? sender, EventArgs e) => this.ButtonClick(this.buttons[2, 0], 2, 0);

            //this.buttons[0, 1] = (Button)this.FindViewById(Resource.Id.button01);
            //this.buttons[0, 1].Click += (object? sender, EventArgs e) => this.ButtonClick(this.buttons[0, 1], 0, 1);
            //this.buttons[1, 1] = (Button)this.FindViewById(Resource.Id.button11);
            //this.buttons[1, 1].Click += (object? sender, EventArgs e) => this.ButtonClick(this.buttons[1, 1], 1, 1);
            //this.buttons[2, 1] = (Button)this.FindViewById(Resource.Id.button21);
            //this.buttons[2, 1].Click += (object? sender, EventArgs e) => this.ButtonClick(this.buttons[2, 1], 2, 1);

            //this.buttons[0, 2] = (Button)this.FindViewById(Resource.Id.button02);
            //this.buttons[0, 2].Click += (object? sender, EventArgs e) => this.ButtonClick(this.buttons[0, 2], 0, 2);
            //this.buttons[1, 2] = (Button)this.FindViewById(Resource.Id.button12);
            //this.buttons[1, 2].Click += (object? sender, EventArgs e) => this.ButtonClick(this.buttons[1, 2], 1, 2);
            //this.buttons[2, 2] = (Button)this.FindViewById(Resource.Id.button22);
            //this.buttons[2, 2].Click += (object? sender, EventArgs e) => this.ButtonClick(this.buttons[2, 2], 2, 2);

            for(int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    this.buttons[x, y].Text = string.Empty;
                }
            }
        }

        private int ToFieldIdx(int x, int y)
        {
            return y * 3 + x;
        }

        private void UpdateBoard()
        {
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    this.buttons[x, y].Text = this.game.Get(this.ToFieldIdx(x, y)).AsString();
                }
            }
        }

        private void ButtonClick(Button button, int x, int y)
        {
            if (this.buttons[x, y].Text == string.Empty)
            {
                this.game.Set(this.ToFieldIdx(x, y));
            }

            this.UpdateBoard();
        }

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            this.InitButtons();

            this.game = new Game(Player.Player);
        }
    }
}