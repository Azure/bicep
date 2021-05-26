# to get the SHA256 checksum, replace the below command with the appropriate version:
# curl -Lf https://github.com/Azure/bicep/releases/download/v0.3.539/bicep-osx-x64 | shasum -a 256

class Bicep < Formula
  desc "Bicep: next generation template language for Azure Resource Manager (ARM)"
  homepage "https://github.com/Azure/bicep"
  version "0.3.539"
  url "https://github.com/Azure/bicep/releases/download/v0.3.539/bicep-osx-x64"
  sha256 "275865ce54bdc737517fdf6c75dbb90413692db48f93941733e66d92a21f31aa"

  license "MIT"

  def install
    bin.install "bicep-osx-x64" => "bicep"
  end

  test do
  end
end
