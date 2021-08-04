# to get the SHA256 checksum, replace the below command with the appropriate version:
# curl -Lf https://github.com/Azure/bicep/releases/download/v0.4.451/bicep-osx-x64 | shasum -a 256

class Bicep < Formula
  desc "Bicep: next generation template language for Azure Resource Manager (ARM)"
  homepage "https://github.com/Azure/bicep"
  version "0.4.451"
  url "https://github.com/Azure/bicep/releases/download/v0.4.451/bicep-osx-x64"
  sha256 "a7d2a019700b09431cc4dc5c37fd686cde6f7e8a3a60f13772bcfff760def983"

  license "MIT"

  def install
    bin.install "bicep-osx-x64" => "bicep"
  end

  test do
  end
end
