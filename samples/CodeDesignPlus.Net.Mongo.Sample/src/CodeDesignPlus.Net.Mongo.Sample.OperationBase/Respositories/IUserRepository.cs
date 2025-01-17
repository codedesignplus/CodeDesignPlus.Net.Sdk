using System;
using CodeDesignPlus.Net.Mongo.Abstractions;
using CodeDesignPlus.Net.Mongo.Abstractions.Operations;
using CodeDesignPlus.Net.Mongo.Sample.OperationBase.Entities;

namespace CodeDesignPlus.Net.Mongo.Sample.OperationBase.Respositories;

public interface IUserRepository: IOperationBase<UserAggregate>
{

}
