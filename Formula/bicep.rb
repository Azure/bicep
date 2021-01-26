# to get the SHA256 checksum, replace the below command with the appropriate version:
# curl -Lf https://github.com/Azure/bicep/releases/download/v0.2.328/bicep-osx-x64 | shasum -a 256

class Bicep < Formula
  desc "Bicep: next generation template language for Azure Resource Manager (ARM)"
  homepage "https://github.com/Azure/bicep"
  version "0.2.328"
  url "https://github.com/Azure/bicep/releases/download/v0.2.328/bicep-osx-x64"
  sha256 "939bd7dfe3be66c8d800de482ed0aba0dc0bf16a9abad564346e597ac0d81c05"

  license "MIT"

  def install
    bin.install "bicep-osx-x64" => "bicep"
  end

  test do
  end
end
