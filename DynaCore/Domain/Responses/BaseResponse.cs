﻿using System.Collections.Generic;
using System.Linq;
using DynaCore.Domain.Dtos;

namespace DynaCore.Domain.Responses
{
    public class BaseResponse
    {
        public bool HasError => Messages.Any(m => m.Type == Constants.MessageTypes.Error);

        public List<MessageDto> Messages { get; set; }

        public BaseResponse()
        {
            Messages = new List<MessageDto>();
        }

        public void AddErrorMessage(string content)
        {
            AddMessage(content, Constants.MessageTypes.Error);
        }

        public void AddInfoMessage(string content)
        {
            AddMessage(content, Constants.MessageTypes.Info);
        }

        public void AddSuccessMessage(string content)
        {
            AddMessage(content, Constants.MessageTypes.Success);
        }

        public void AddWarningMessage(string content)
        {
            AddMessage(content, Constants.MessageTypes.Warning);
        }

        private void AddMessage(string content, string type)
        {
            Messages.Add(new MessageDto(content, type));
        }
    }
}