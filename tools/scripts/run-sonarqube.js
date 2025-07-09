// tools/scripts/run-sonarqube.js

const { execSync } = require('child_process');

const sonarHost = process.env.SONAR_HOST_URL;
const sonarToken = process.env.SONAR_TOKEN;

if (!sonarHost || !sonarToken) {
    console.error('❌ ERROR: Required environment variables are missing.');
    console.error('   Make sure SONAR_HOST_URL and SONAR_TOKEN are defined in your system or in a .env file');
    process.exit(1);
}

const args = process.argv.slice(2).reduce((acc, arg) => {
    const [key, value] = arg.split('=');
    acc[key.replace(/^--/, '')] = value;
    return acc;
}, {});


const exclusions = {
    'CodeDesignPlus.Net.Cache': "**/CodeDesignPlus.Net.Core/**,**/CodeDesignPlus.Net.Redis/**,**/CodeDesignPlus.Net.Serializers/**,**Tests*.cs",
    'CodeDesignPlus.Net.Core': "**/CodeDesignPlus.Net.Serializers/**,**Tests*.cs",
    'CodeDesignPlus.Net.Criteria': "**/CodeDesignPlus.Net.Core/**,**Tests*.cs",
    'CodeDesignPlus.Net.EFCore': "**/CodeDesignPlus.Net.Core**,**/CodeDesignPlus.Net.Redis**,**/CodeDesignPlus.Net.Security**,**/CodeDesignPlus.Net.Serializers**,**/CodeDesignPlus.Abstractions/**/*.cs,**/CodeDesignPlus.Entities/**/*.cs,**/CodeDesignPlus.InMemory/**/*.cs,**Tests*.cs",
    'CodeDesignPlus.Net.Event.Sourcing': "**/CodeDesignPlus.Net.Core/**,**/CodeDesignPlus.Net.xUnit/**,**/CodeDesignPlus.Net.Redis/**,**/CodeDesignPlus.Net.Serializers/**,**Tests*.cs",
    'CodeDesignPlus.Net.EventStore': "**/CodeDesignPlus.Net.Core/**,**/CodeDesignPlus.Net.Event.Sourcing/**,**/CodeDesignPlus.Net.Redis/**,**/CodeDesignPlus.Net.Serializers/**,**Tests*.cs",
    'CodeDesignPlus.Net.EventStore.PubSub': "**/CodeDesignPlus.Net.Core/**,**/CodeDesignPlus.Net.Event.Sourcing/**,**/CodeDesignPlus.Net.EventStore/**,**/CodeDesignPlus.Net.Exceptions/**,**/CodeDesignPlus.Net.PubSub/**,**/CodeDesignPlus.Net.Redis/**,**/CodeDesignPlus.Net.Serializers/**,**Tests*.cs",
    'CodeDesignPlus.Net.Exceptions': ",**Tests*.cs",
    'CodeDesignPlus.Net.File.Storage': "**/CodeDesignPlus.Net.Core/**,**/CodeDesignPlus.Net.Redis/**,**/CodeDesignPlus.Net.Security/**,**/CodeDesignPlus.Net.Serializers/**,**Tests*.cs",
    'CodeDesignPlus.Net.Generator': "**Tests*.cs",
    'CodeDesignPlus.Net.gRpc.Clients': "**Tests*.cs",
    'CodeDesignPlus.Net.Kafka': "**/CodeDesignPlus.Net.Core/**,**/CodeDesignPlus.Net.PubSub/**,**/CodeDesignPlus.Net.Redis/**,**/CodeDesignPlus.Net.Serializers/**,**Tests*.cs",
    'CodeDesignPlus.Net.Logger': "**/CodeDesignPlus.Net.Core/**,**/CodeDesignPlus.Net.Redis/**,**/CodeDesignPlus.Net.Serializers/**,**Tests*.cs",
    'CodeDesignPlus.Net.Microservice.Commons': "**/CodeDesignPlus.Net.Core/**,**/CodeDesignPlus.Net.Exceptions/**,**/CodeDesignPlus.Net.Serializers/**,**Tests*.cs",
    'CodeDesignPlus.Net.Mongo': "**/CodeDesignPlus.Net.Core/**,**/CodeDesignPlus.Net.Criteria/**,**/CodeDesignPlus.Net.Mongo.Diagnostics/**,**/CodeDesignPlus.Net.Redis/**,**/CodeDesignPlus.Net.Security/**,**/CodeDesignPlus.Net.Serializers/**,**Tests*.cs",
    'CodeDesignPlus.Net.Observability': "**/CodeDesignPlus.Net.Core/**,**/CodeDesignPlus.Net.Redis/**,**/CodeDesignPlus.Net.Serializers/**,**Tests*.cs",
    'CodeDesignPlus.Net.PubSub': "**/CodeDesignPlus.Net.Core/**,**/CodeDesignPlus.Net.Redis/**,**/CodeDesignPlus.Net.Serializers/**,**Tests*.cs",
    'CodeDesignPlus.Net.RabbitMQ': "**/CodeDesignPlus.Net.Core/**,**/CodeDesignPlus.Net.Redis/**,**/CodeDesignPlus.Net.Exceptions/**,**/CodeDesignPlus.Net.PubSub/**,**/CodeDesignPlus.Net.Serializers/**,**Tests*.cs",
    'CodeDesignPlus.Net.Redis': "**/CodeDesignPlus.Net.Core/**,**/CodeDesignPlus.Net.Serializers/**,**Tests*.cs",
    'CodeDesignPlus.Net.Redis.Cache': "**/CodeDesignPlus.Net.Core/**,**/CodeDesignPlus.Net.Redis/**,**/CodeDesignPlus.Net.Serializers/**,**Tests*.cs",
    'CodeDesignPlus.Net.Redis.PubSub': "**/CodeDesignPlus.Net.Core/**,**/CodeDesignPlus.Net.Serializers/**,**/CodeDesignPlus.Net.PubSub/**,**/CodeDesignPlus.Net.Redis/**,**Tests*.cs",
    'CodeDesignPlus.Net.Security': "**/CodeDesignPlus.Net.Core/**,**/CodeDesignPlus.Net.Serializers/**,**/CodeDesignPlus.Net.Redis/**,**Tests*.cs",
    'CodeDesignPlus.Net.Serializers': "**/CodeDesignPlus.Net.Core/**,**Tests*.cs",
    'CodeDesignPlus.Net.Vault': "**/CodeDesignPlus.Net.Core/**,**/CodeDesignPlus.Net.Serializers/**,**/CodeDesignPlus.Net.Redis/**,**Tests*.cs",
    'CodeDesignPlus.Net.xUnit': "**Tests*.cs",
    'CodeDesignPlus.Net.xUnit.Microservice': "**/CodeDesignPlus.Net.Core/**,**/CodeDesignPlus.Net.Exceptions/**,**/CodeDesignPlus.Net.Security/**,**/CodeDesignPlus.Net.Serializers/**,**Tests*.cs"
}

const org = args.org;
const projectKey = args.project;
const projectRoot = args.projectRoot;

if (!org || !projectKey || !projectRoot) {
    console.error('❌ ERROR: Missing arguments --org, --project, or --projectRoot in the script call.');
    process.exit(1);
}

console.log(`\n🔍 Starting SonarQube analysis for project: ${projectKey} in organization: ${org}`);
console.log(`\n Exclusions for project ${projectKey}: ${exclusions[projectKey] || 'None'}`);

const joinedCommand =
    `dotnet test ${projectRoot}/${projectKey}.sln /p:CollectCoverage=true /p:CoverletOutputFormat=opencover && ` +
    `dotnet sonarscanner begin /o:${org} /k:${projectKey} /d:sonar.host.url=${sonarHost} /d:sonar.cs.opencover.reportsPaths=${projectRoot}/tests/${projectKey}.Test/coverage.opencover.xml /d:sonar.javascript.enabled=false /d:sonar.architecture.enabled=false /d:sonar.coverage.exclusions=\"${exclusions[projectKey]}\" /d:sonar.login=${sonarToken} && ` +
    `dotnet build ${projectRoot}/${projectKey}.sln && ` +
    `dotnet sonarscanner end /d:sonar.token=${sonarToken}`;

try {

    execSync(joinedCommand, { stdio: 'inherit' });
    console.log('\n✅ SonarQube analysis completed successfully.');
    
} catch (error) {
    console.error('\n❌ ERROR: A command failed during SonarQube analysis.');
    if (error instanceof Error) {
        console.error('Error details:', error.message);
        if (error.stack) {
            console.error(error.stack);
        }
    } else {
        console.error('Unknown error:', error);
    }
    process.exit(1);
}