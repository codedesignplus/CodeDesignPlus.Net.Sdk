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

const org = args.org;
const projectKey = args.project;
const projectRoot = args.projectRoot;

if (!org || !projectKey || !projectRoot) {
  console.error('❌ ERROR: Missing arguments --org, --project, or --projectRoot in the script call.');
  process.exit(1);
}

const commands = [
  `dotnet test ${projectRoot}/${projectKey}.sln /p:CollectCoverage=true /p:CoverletOutputFormat=opencover`,
  `dotnet sonarscanner begin /o:${org} /k:${projectKey} /d:sonar.host.url=${sonarHost} /d:sonar.cs.opencover.reportsPaths=${projectRoot}/tests/${projectKey}.Test/coverage.opencover.xml /d:sonar.coverage.exclusions="**/CodeDesignPlus.Net.Core/**,**/CodeDesignPlus.Net.Redis/**,**/CodeDesignPlus.Net.Serializers/**,**Tests*.cs" /d:sonar.login=${sonarToken}`,
  `dotnet build ${projectRoot}/${projectKey}.sln`,
  `dotnet sonarscanner end /d:sonar.token=${sonarToken}`
];

try {
  for (const command of commands) {
    console.log(`\n▶️  Running: ${command}\n`);
    execSync(command, { stdio: 'inherit' }); 
  }
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