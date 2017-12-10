﻿using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FoosLiveAndroid.Model.Interface;

namespace FoosLiveAndroid.Fragments
{
    public class HistoryListAdapter : RecyclerView.Adapter
    {
        private List<IHistory> _historyList;
        public HistoryListAdapter(List<IHistory> historyList)
        {
            _historyList = historyList;
        }

        /// <summary>
        /// Gets the reference of each item from the list
        /// </summary>
        private class HistoryListViewHolder : RecyclerView.ViewHolder
        {
            internal readonly TextView Team1Name;
            internal readonly TextView Score;
            internal readonly TextView Team2Name;

            public HistoryListViewHolder(View v) : base(v)
            {
                Team1Name = v.FindViewById<TextView>(Resource.Id.history_team1Name);
                Score = v.FindViewById<TextView>(Resource.Id.history_score);
                Team2Name = v.FindViewById<TextView>(Resource.Id.history_team2Name);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Loads layout
            const int id = Resource.Layout.fragment_history_item;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            return new HistoryListViewHolder(itemView);
        }

        /// Updates view content on load/scroll
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = _historyList[position];

            if (!(holder is HistoryListViewHolder viewHolder)) return;
            viewHolder.Team1Name.Text = item.BlueTeamName;
            viewHolder.Score.Text = $"{item.BlueTeamPoints}  :  {item.RedTeamPoints}";
            viewHolder.Team2Name.Text = item.RedTeamName;
        }

        public override int ItemCount => _historyList.Count;
    }
}